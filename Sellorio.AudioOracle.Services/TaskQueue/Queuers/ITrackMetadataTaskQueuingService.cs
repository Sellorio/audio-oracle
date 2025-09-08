using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.TaskQueue.Queuers;

public interface ITrackMetadataTaskQueuingService
{
    Task QueueAsync(int trackId);
}
