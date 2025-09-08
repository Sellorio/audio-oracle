using Sellorio.AudioOracle.Data.TaskQueue;
using Sellorio.AudioOracle.Library.Mapping;
using Sellorio.AudioOracle.Models.TaskQueue;

namespace Sellorio.AudioOracle.Services.TaskQueue;

internal class TaskQueueMapper : StaticMapperBase<TaskQueueMapper>, ITaskQueueMapper
{
    public QueuedTask Map(QueuedTaskData from) => Map<QueuedTask>(from);
}
