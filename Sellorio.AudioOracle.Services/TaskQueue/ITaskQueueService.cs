using System.Threading;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.TaskQueue;

public interface ITaskQueueService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}