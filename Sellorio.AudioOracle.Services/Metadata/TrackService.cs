using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.TaskQueue.Queuers;

namespace Sellorio.AudioOracle.Services.Metadata;

internal class TrackService(
    DatabaseContext databaseContext,
    ITrackMetadataTaskQueuingService trackMetadataTaskQueuingService,
    IDownloadTrackTaskQueuingService downloadTrackTaskQueuingService) : ITrackService
{
    public async Task<Result> RetryAllTracksAsync(int albumId)
    {
        var tracks =
            await databaseContext.Tracks
                .Where(x => x.AlbumId == albumId)
                .Where(x => x.Status == TrackStatus.MetadataRetrievalFailed || x.Status == TrackStatus.DownloadFailed)
                .ToArrayAsync();

        foreach (var track in tracks)
        {
            if (track.Status == TrackStatus.MetadataRetrievalFailed)
            {
                track.Status = TrackStatus.MissingMetadata;
            }
            else if (track.Status == TrackStatus.DownloadFailed)
            {
                track.Status = TrackStatus.Downloading;
            }
            else
            {
                continue;
            }

            track.StatusText = null;
        }

        await databaseContext.SaveChangesAsync();

        foreach (var track in tracks)
        {
            if (track.Status == TrackStatus.MissingMetadata)
            {
                await trackMetadataTaskQueuingService.QueueAsync(track.Id);
            }
            else if (track.Status == TrackStatus.Downloading)
            {
                await downloadTrackTaskQueuingService.QueueAsync(track.Id);
            }
            else
            {
                continue;
            }
        }

        return Result.Success();
    }
}
