using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Library;
using Sellorio.AudioOracle.Library.ApiTools;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class TrackMetadataProvider(IApiService apiService, IBrowseService browseService, IYouTubeAlbumMetadataProvider albumMetadataProvider, ITrackIdsResolver trackIdsResolver) : IYouTubeTrackMetadataProvider
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _cacheDuration = TimeSpan.MaxValue;

    public string ProviderName => Constants.ProviderName;

    public Task<ValueResult<TrackMetadata>> GetTrackMetadataAsync(ResolvedIds albumIds, ResolvedIds trackIds)
    {
        return ProviderHelper.GetWithCacheAsync(_cache, _cacheDuration, (albumIds, trackIds), GetTrackMetadataWithoutCacheAsync);
    }

    private async Task<ValueResult<TrackMetadata>> GetTrackMetadataWithoutCacheAsync((ResolvedIds AlbumIds, ResolvedIds TrackIds) inputs)
    {
        var albumMetadata = await albumMetadataProvider.GetAlbumMetadataAsync(inputs.AlbumIds);

        var albumMetadataTrack = albumMetadata.Value.Tracks.First(x => x.Ids.SourceId == inputs.TrackIds.SourceId);

        var nextApiResult = await apiService.PostWithContextAsync("/next?prettyPrint=false", new { videoId = inputs.TrackIds.SourceId, playlistId = inputs.AlbumIds.SourceId });
        var playerApiResult = await apiService.PostWithContextAsync("/player?prettyPrint=false", new { videoId = inputs.TrackIds.SourceId });

        var infoSection = nextApiResult["contents"]!["singleColumnMusicWatchNextResultsRenderer"]!["tabbedRenderer"]!["watchNextTabbedResultsRenderer"]!["tabs"]![0]!["tabRenderer"]!["content"]!["musicQueueRenderer"]!["content"]!["playlistPanelRenderer"]!["contents"]![0]!["playlistPanelVideoRenderer"]!;
        var byLineSection = infoSection["longBylineText"]!["runs"]!;
        var videoDetails = playerApiResult["videoDetails"]!;

        var (title, alternateTitle) = GetTitles(albumMetadataTrack.Title, playerApiResult);
        var duration = TimeSpan.FromSeconds(int.Parse(videoDetails.Get<string>("lengthSeconds")!));

        var artistIds = new List<ResolvedIds>(3);

        foreach (var artistElement in byLineSection.AsEnumerable().Take(byLineSection.ArrayLength - 3).SkipEvery(2))
        {
            artistIds.Add(await GetArtistIdFromJsonAsync(inputs.AlbumIds, artistElement!));
        }

        var trackNumber = albumMetadata.Value.Tracks.IndexOf(albumMetadataTrack) + 1;

        // make sure we apply any automatic redirects when downloading tracks
        var trackIdResult = await trackIdsResolver.GetLatestIdAsync(inputs.TrackIds.SourceId);
        var downloadId = trackIdResult.WasSuccess ? trackIdResult.Value : inputs.TrackIds.SourceId;

        return new TrackMetadata
        {
            AlbumArtOverrideUrl = null,
            ArtistIds = artistIds,
            DownloadIds = new() { SourceId = downloadId, SourceUrlId = downloadId },
            Duration = duration,
            TrackNumber = trackNumber,
            Title = title,
            AlternateTitle = alternateTitle
        };
    }

    private async Task<ResolvedIds> GetArtistIdFromJsonAsync(ResolvedIds albumId, JsonNavigator artistElement)
    {
        var navigationEndpoint = artistElement["navigationEndpoint"];

        if (navigationEndpoint == null)
        {
            var artistName = artistElement.Get<string>("text")!.Trim();
            var unregisteredArtistId = $"{Constants.UnregisteredArtistIdPrefix}:{albumId.SourceId}:{artistName}";
            return new() { SourceId = unregisteredArtistId, SourceUrlId = unregisteredArtistId };
        }
        else
        {
            var browseId = navigationEndpoint["browseEndpoint"]!.Get<string>("browseId")!;
            var artistId = await browseService.ResolveChannelIdFromBrowseIdAsync(browseId);
            return new() { SourceId = artistId, SourceUrlId = artistId };
        }
    }

    private static (string Title, string? AlternateTitle) GetTitles(string fullTitle, JsonNavigator apiResult)
    {
        if (!fullTitle.Contains(" - "))
        {
            return (fullTitle, null);
        }

        var possibleAlternateTitle = apiResult["videoDetails"]!.Get<string>("title");

        var indexOfSeparator = fullTitle.IndexOf(" - ");
        var title1 = fullTitle[..indexOfSeparator];
        var title2 = fullTitle[(indexOfSeparator + 3)..];

        return
            possibleAlternateTitle == title2
                ? (title1, title2)
                : (fullTitle, null);
    }
}
