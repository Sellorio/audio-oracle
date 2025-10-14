using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Library.ApiTools;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

internal class AlbumMetadataProvider(IApiService apiService, IBrowseService browseService, IPageService pageService) : IAlbumMetadataProvider, IYouTubeAlbumMetadataProvider
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _cacheDuration = TimeSpan.MaxValue;

    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<AlbumMetadata>> GetAlbumMetadataAsync(ResolvedIds resolvedIds)
    {
        return await ProviderHelper.GetWithCacheAsync(_cache, _cacheDuration, resolvedIds, GetAlbumMetadataWithoutCacheAsync);
    }

    private async Task<ValueResult<AlbumMetadata>> GetAlbumMetadataWithoutCacheAsync(ResolvedIds resolvedIds)
    {
        var browseId = resolvedIds.SourceUrlId;
        var apiResult = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { browseId });
        var contents = apiResult["contents"];

        if (contents == null)
        {
            return ResultMessage.NotFound("Album");
        }

        var albumInfoSection = contents["twoColumnBrowseResultsRenderer"]!["tabs"]![0]!["tabRenderer"]!["content"]!["sectionListRenderer"]!["contents"]![0]!["musicResponsiveHeaderRenderer"]!;

        var playlistType = albumInfoSection["subtitle"]!["runs"]![0]!.Get<string>("text");

        var albumArt = albumInfoSection["thumbnail"]!["musicThumbnailRenderer"]!["thumbnail"]!["thumbnails"]!.NthFromLast(0)!.Get<string>("url");
        var albumTitle = albumInfoSection["title"]!["runs"]![0]!.Get<string>("text")!;
        var releaseYearString = albumInfoSection["subtitle"]!["runs"]!.NthFromLast(0)!.Get<string>("text")!;
        var releaseYear = int.Parse(releaseYearString);

        var artists = new List<ResolvedIds>(2);

        var artistsSection = albumInfoSection["straplineTextOne"]!["runs"]!;

        for (var i = 0; i < artistsSection.ArrayLength; i += 2)
        {
            var navigationEndpoint = artistsSection[i]!["navigationEndpoint"];

            if (navigationEndpoint == null)
            {
                var artistName = artistsSection[i]!.Get<string>("text")!.Trim();
                var unregisteredArtistId = $"{Constants.UnregisteredArtistIdPrefix}:{resolvedIds.SourceId}:{artistName}";
                artists.Add(new() { SourceId = unregisteredArtistId, SourceUrlId = unregisteredArtistId });
            }
            else
            {
                var artistBrowseId = navigationEndpoint["browseEndpoint"]!.Get<string>("browseId")!;
                var artistId = await browseService.ResolveChannelIdFromBrowseIdAsync(artistBrowseId);
                artists.Add(new() { SourceId = artistId, SourceUrlId = artistId });
            }
        }

        // FYI: private uploads do not appear in lists returned by the browse endpoint
        // This is OK since we don't intend to import those from a youtube playlist anyway
        var trackElements = (await GetTrackElementsWithExpandedContinuationsAsync(resolvedIds)).ToArray();
        var tracks = new AlbumTrackMetadata[trackElements.Length];

        for (var i = 0; i < trackElements.Length; i++)
        {
            var trackId = trackElements[i]!["musicResponsiveListItemRenderer"]!["playlistItemData"]!.Get<string>("videoId")!;
            var trackTitle = trackElements[i]!["musicResponsiveListItemRenderer"]!["flexColumns"]![0]!["musicResponsiveListItemFlexColumnRenderer"]!["text"]!["runs"]![0]!.Get<string>("text")!;

            tracks[i] = new AlbumTrackMetadata
            {
                Ids = new ResolvedIds() { SourceId = trackId, SourceUrlId = trackId },
                Title = trackTitle
            };
        }

        return new AlbumMetadata
        {
            AlbumArtUrl = albumArt,
            ArtistIds = artists,
            Tracks = tracks,
            ReleaseDate = null,
            ReleaseYear = (ushort)releaseYear,
            Title = albumTitle
        };
    }

    private async Task<IEnumerable<JsonNavigator?>> GetTrackElementsWithExpandedContinuationsAsync(ResolvedIds resolvedIds)
    {
        var pageData = await pageService.GetPageInitialDataAsync("https://music.youtube.com/playlist?list=" + resolvedIds.SourceId);
        var data = pageData[1];
        var itemsParent = data["contents"]!["twoColumnBrowseResultsRenderer"]!["secondaryContents"]!["sectionListRenderer"]!["contents"]![0]!;
        var items = (itemsParent["musicShelfRenderer"] ?? itemsParent["musicPlaylistShelfRenderer"])!["contents"]!;
        var continuationItemRenderer = items.NthFromLast(0)?["continuationItemRenderer"];

        var trackElements = items.AsEnumerable();

        if (continuationItemRenderer != null)
        {
            var continuationToken = continuationItemRenderer["continuationEndpoint"]!["continuationCommand"]!.Get<string>("token")!;
            trackElements = trackElements.Take(items.ArrayLength - 1).Concat(await GetContinuationTracksAsync(continuationToken));
        }

        return trackElements;
    }

    private async Task<IEnumerable<JsonNavigator?>> GetContinuationTracksAsync(string continuationToken)
    {
        var continuationApiResult = await apiService.PostWithContextAsync("/browse?prettyPrint=false", new { continuation = continuationToken });
        var items = continuationApiResult["onResponseReceivedActions"]![0]!["appendContinuationItemsAction"]!["continuationItems"]!;

        IEnumerable<JsonNavigator?> result = items.Where(x => x["musicResponsiveListItemRenderer"] != null).ToArray();

        foreach (var continuation in items.Select(x => x["continuationItemRenderer"]).Where(x => x != null))
        {
            var nextContinuationToken = continuation!["continuationEndpoint"]!["continuationCommand"]!.Get<string>("token")!;
            result = result.Concat(await GetContinuationTracksAsync(nextContinuationToken));
        }

        return result;
    }
}
