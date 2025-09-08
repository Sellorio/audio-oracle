using System.Threading.Channels;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.TaskQueue;
using Sellorio.AudioOracle.Models.TaskQueue;
using Sellorio.AudioOracle.Services.TaskQueue.Handlers;

namespace Sellorio.AudioOracle.Services.TaskQueue;

internal class TaskQueuingService(DatabaseContext databaseContext, Channel<QueuedTask> taskChannel, ITaskQueueMapper mapper) : ITaskQueuingService
{
    public async Task<QueuedTask> QueueTaskAsync<TTaskHandler>(int? objectId, int? objectId2)
        where TTaskHandler : ITaskHandler
    {
        var task = new QueuedTaskData
        {
            HandlerName = typeof(TTaskHandler).Name,
            ObjectId = objectId,
            ObjectId2 = objectId2
        };

        databaseContext.QueuedTasks.Add(task);
        await databaseContext.SaveChangesAsync();

        var result = mapper.Map(task);

        await taskChannel.Writer.WriteAsync(result);

        return result;
    }
}
