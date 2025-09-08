using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.TaskQueue.Queuers;

internal interface IDownloadTrackTaskQueuingService
{
    Task QueueAsync(int trackId);
}
