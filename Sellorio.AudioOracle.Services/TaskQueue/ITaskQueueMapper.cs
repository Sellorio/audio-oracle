using Sellorio.AudioOracle.Data.TaskQueue;
using Sellorio.AudioOracle.Models.TaskQueue;

namespace Sellorio.AudioOracle.Services.TaskQueue;

public interface ITaskQueueMapper
{
    QueuedTask Map(QueuedTaskData from);
}