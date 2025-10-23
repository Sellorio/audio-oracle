using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.TaskQueue.Queuers;

internal interface IRefreshTrackTagsTaskQueuingService
{
    Task QueueAsync(int trackId);
}
