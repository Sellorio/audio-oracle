using Sellorio.AudioOracle.Data.TaskQueue;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.TaskQueue;

namespace Sellorio.AudioOracle.Services.TaskQueue;

public interface ITaskQueueMapper : IMap<QueuedTaskData, QueuedTask>
{
}