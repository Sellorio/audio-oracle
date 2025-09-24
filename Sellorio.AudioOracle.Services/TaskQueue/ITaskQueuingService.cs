using System.Threading.Tasks;
using Sellorio.AudioOracle.Models.TaskQueue;
using Sellorio.AudioOracle.Services.TaskQueue.Handlers;

namespace Sellorio.AudioOracle.Services.TaskQueue;

internal interface ITaskQueuingService
{
    Task<QueuedTask?> QueueTaskAsync<TTaskHandler>(int? objectId, int? objectId2)
        where TTaskHandler : ITaskHandler;
}
