using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;
using Sellorio.AudioOracle.Providers.MusicBrainz.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal class AlbumMetadataProvider(HttpClient httpClient, ICoverArtArchiveService coverArtArchiveService)
    : IAlbumMetadataProvider, IMusicBrainzAlbumMetadataProvider
{
    private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<AlbumMetadata>> GetAlbumMetadataAsync(ResolvedIds resolvedIds)
    {
        var release =
            await ProviderHelper.GetWithCacheAndRateLimitAsync(
                _cache,
                _cacheDuration,
                RateLimiters.MusicBrainz,
                Guid.Parse(resolvedIds.SourceId),
                GetReleaseAsync);

        return new AlbumMetadata
        {
            AlbumArtUrl = await coverArtArchiveService.GetReleaseArtUrlAsync(Guid.Parse(resolvedIds.SourceUrlId)),
            ArtistIds = release!.ArtistCredit.Select(x => x.Artist.Id.ToString()).Select(x => new ResolvedIds { SourceId = x, SourceUrlId = x }).ToArray(),
            ReleaseDate = release.Date,
            ReleaseYear = (ushort?)release.ReleaseYear,
            Title = release.Title,
            TrackCount = (ushort)release.TrackCount,
            Tracks =
                release.Media
                    .SelectMany(x => x.Tracks!)
                    .Select(x => new AlbumTrackMetadata
                    {
                        Ids = new() { SourceId = x.Id.ToString(), SourceUrlId = x.Id.ToString() },
                        Title = x.Title
                    })
                    .ToArray()
        };
    }

    public async Task<ReleaseDto?> GetMusicBrainzReleaseAsync(Guid id)
    {
        return
            await ProviderHelper.GetWithCacheAndRateLimitAsync(
                _cache,
                _cacheDuration,
                RateLimiters.MusicBrainz,
                id,
                GetReleaseAsync);
    }

    public Task<ValueResult<ResolvedIds>> ResolveAlbumIdsAsync(string sourceUrlId)
    {
        return Task.FromResult(ValueResult.Success(new ResolvedIds
        {
            SourceId = sourceUrlId,
            SourceUrlId = sourceUrlId
        }));
    }

    private async Task<ReleaseDto?> GetReleaseAsync(Guid id)
    {
        var responseMessage = await httpClient.GetAsync($"release/{id}?fmt=json&inc=artists+media+recordings");

        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        responseMessage.EnsureSuccessStatusCode();

        var json = await responseMessage.Content.ReadAsStringAsync();
        var release = JsonSerializer.Deserialize<ReleaseDto>(json, Constants.JsonOptions)!;

        var trackOffset = 0;

        foreach (var medium in release.Media)
        {
            medium.TrackOffset = trackOffset;

            foreach (var track in medium.Tracks!)
            {
                track.Number = (int.Parse(track.Number) + trackOffset).ToString();
            }

            trackOffset += medium.TrackCount;
        }

        release.TrackCount = trackOffset;

        return release;
    }
}
