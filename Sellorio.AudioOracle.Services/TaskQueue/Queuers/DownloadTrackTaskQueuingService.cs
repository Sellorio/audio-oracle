using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.Metadata;

namespace Sellorio.AudioOracle.Services.TaskQueue.Queuers;

internal class DownloadTrackTaskQueuingService(DatabaseContext databaseContext, ITaskQueuingService taskQueuingService) : IDownloadTrackTaskQueuingService
{
    public async Task QueueAsync(int trackId)
    {
        var trackEntry = databaseContext.ChangeTracker.Entries<TrackData>().FirstOrDefault(x => x.Entity.Id == trackId);

        if (trackEntry != null && trackEntry.State == EntityState.Deleted ||
            trackEntry == null && !await databaseContext.Tracks.AnyAsync(x => x.Id == trackId))
        {
            throw new ArgumentException("Track Id does not exist.", nameof(trackId));
        }

        await taskQueuingService.QueueTaskAsync<Handlers.DownloadTrack>(trackId, null);
    }
}
