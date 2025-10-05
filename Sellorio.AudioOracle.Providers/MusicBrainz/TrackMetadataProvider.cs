using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

internal class TrackMetadataProvider(IMusicBrainzAlbumMetadataProvider musicBrainzAlbumMetadataProvider)
    : ITrackMetadataProvider
{
    public string ProviderName => Constants.ProviderName;

    public async Task<ValueResult<TrackMetadata>> GetTrackMetadataAsync(ResolvedIds albumIds, ResolvedIds trackIds)
    {
        var releaseId = Guid.Parse(albumIds.SourceId);
        var trackId = Guid.Parse(trackIds.SourceId);

        var release = await musicBrainzAlbumMetadataProvider.GetMusicBrainzReleaseAsync(releaseId);
        var track = release!.Media!.SelectMany(x => x.Tracks!).First(x => x.Id == trackId);
        var recording = track.Recording!;

        return new TrackMetadata
        {
            AlbumArtOverrideUrl = null,
            Title = track.Title,
            AlternateTitle = null,
            DownloadIds = null,
            ArtistIds = track.ArtistCredit?.Select(x => x.Artist.Id.ToString()).Select(x => new ResolvedIds { SourceId = x, SourceUrlId = x }).ToArray() ?? [],
            Duration = track.Length == null ? null : TimeSpan.FromMilliseconds(track.Length.Value),
            TrackNumber = int.Parse(track.Number)
        };
    }
}
