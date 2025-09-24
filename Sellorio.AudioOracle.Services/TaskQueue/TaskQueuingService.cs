using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Data.TaskQueue;
using Sellorio.AudioOracle.Models.TaskQueue;
using Sellorio.AudioOracle.Services.TaskQueue.Handlers;

namespace Sellorio.AudioOracle.Services.TaskQueue;

internal class TaskQueuingService(
    DatabaseContext databaseContext,
    Channel<QueuedTask> taskChannel,
    ITaskQueueMapper mapper,
    ILogger<TaskQueuingService> logger) : ITaskQueuingService
{
    public async Task<QueuedTask?> QueueTaskAsync<TTaskHandler>(int? objectId, int? objectId2)
        where TTaskHandler : ITaskHandler
    {
        try
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
        catch (Exception ex)
        {
            logger.LogException(ex, "Failed to queue task.");
            return null;
        }
    }
}
