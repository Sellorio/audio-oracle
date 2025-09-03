using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.Models;
using Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal class TrackMetadataProvider(
    HttpClient httpClient,
    IMusicBrainzAlbumMetadataProvider musicBrainzAlbumMetadataProvider)
    : ITrackMetadataProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<TrackMetadata>> GetTrackMetadataAsync(ResolvedIds albumIds, ResolvedIds trackIds)
    {
        var releaseId = Guid.Parse(albumIds.SourceId);
        var trackId = Guid.Parse(trackIds.SourceId);

        HttpResponseMessage? responseMessage = null;

        await RateLimiters.MusicBrainz.WithRateLimit(async () =>
            responseMessage = await httpClient.GetAsync($"recording/{trackIds.SourceUrlId}?fmt=json&inc=releases+artists+media"));

        if (responseMessage!.StatusCode == HttpStatusCode.NotFound)
        {
            return ResultMessage.Error("Track not found.");
        }

        responseMessage.EnsureSuccessStatusCode();

        var json = await responseMessage.Content.ReadAsStringAsync();
        var recording = JsonSerializer.Deserialize<RecordingDto>(json, Constants.JsonOptions);
        var recordingRelease = recording!.Releases.First(x => x.Id == releaseId);
        var recordingReleaseTrack = recordingRelease.Media.SelectMany(x => x.Tracks!).Single();

        var release = await musicBrainzAlbumMetadataProvider.GetMusicBrainzReleaseAsync(releaseId);
        var track = release!.Media.SelectMany(x => x.Tracks!).First(x => x.Id == recordingReleaseTrack.Id);

        return new TrackMetadata
        {
            AlbumArtOverrideUrl = null, // musicbrainz doesn't support per-track album art, youtube does
            Title = track.Title,
            AlternateTitle = null,
            DownloadIds = null,
            ArtistIds = recording.ArtistCredit.Select(x => x.Artist.Id.ToString()).Select(x => new ResolvedIds { SourceId = x, SourceUrlId = x }).ToArray(),
            Duration = recording.Length == null ? null : TimeSpan.FromMilliseconds(recording.Length.Value),
            TrackNumber = int.Parse(track.Number)
        };
    }

    public Task<ValueResult<ResolvedIds>> ResolveTrackIdsAsync(string sourceUrlId)
    {
        return Task.FromResult(ValueResult.Success(new ResolvedIds
        {
            SourceUrlId = sourceUrlId,
            SourceId = sourceUrlId
        }));
    }
}
