using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Data;
using Sellorio.AudioOracle.Models.TaskQueue;
using Sellorio.AudioOracle.Services.TaskQueue.Handlers;

namespace Sellorio.AudioOracle.Services.TaskQueue;

internal class TaskQueueService(IServiceProvider serviceProvider, ILogger<TaskQueueService> logger, Channel<QueuedTask> channel) : ITaskQueueService
{
    private readonly List<QueuedTask> _futureTasks = new(10);

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await LoadSavedTasksAsync(cancellationToken);

        while (true)
        {
            var queuedTask = await WaitForTaskAsync(cancellationToken);

            using var scope = serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<ITaskHandler>>();
            var handler = handlers.FirstOrDefault(x => x.HandlerName == queuedTask.HandlerName);
            var taskData = await databaseContext.QueuedTasks.FindAsync([queuedTask.Id], cancellationToken);

            if (taskData == null)
            {
                continue;
            }

            if (handler == null)
            {
                logger.LogWarning("The handler for {Handler} no longer exists. The task has been deleted.", queuedTask.HandlerName);
                databaseContext.Remove(taskData);
                await databaseContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                var handlerContext = new TaskHandlerContext
                {
                    ObjectId = queuedTask.ObjectId,
                    ObjectId2 = queuedTask.ObjectId2,
                    CancellationToken = cancellationToken
                };

                try
                {
                    await handler.HandleAsync(handlerContext);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Failed to execute task {Handler}.", handler.HandlerName);

                    if (taskData.Lives > 0)
                    {
                        taskData.Lives--;
                    }

                    if (taskData.Lives <= 0)
                    {
                        taskData.QueuedAt = null;
                    }
                    else
                    {
                        taskData.QueuedAt = DateTime.UtcNow.AddHours(1);
                        _futureTasks.Add(queuedTask);
                    }

                    await databaseContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }

    private async Task LoadSavedTasksAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var taskQueueMapper = scope.ServiceProvider.GetRequiredService<ITaskQueueMapper>();

        var data = await databaseContext.QueuedTasks.AsNoTracking().OrderBy(x => x.QueuedAt).ToListAsync(cancellationToken);
        var tasks = data.Select(taskQueueMapper.Map).ToArray();

        foreach (var task in tasks)
        {
            await channel.Writer.WriteAsync(task, cancellationToken);
        }
    }

    private async Task<QueuedTask> WaitForTaskAsync(CancellationToken cancellationToken)
    {
        while (channel.Reader.Count != 0)
        {
            var queuedTask = await channel.Reader.ReadAsync(cancellationToken);

            if (queuedTask.QueuedAt < DateTime.UtcNow)
            {
                return queuedTask;
            }

            _futureTasks.Add(queuedTask);
        }

        var nextFutureTask = _futureTasks.Where(x => x.QueuedAt != null).OrderBy(x => x.QueuedAt).FirstOrDefault();

        if (nextFutureTask == null)
        {
            await channel.Reader.WaitToReadAsync(cancellationToken);
        }
        else if (nextFutureTask.QueuedAt - TimeSpan.FromSeconds(5) < DateTime.UtcNow)
        {
            _futureTasks.Remove(nextFutureTask);
            return nextFutureTask;
        }
        else
        {
            // waits for next future task or next item to come through the channel
            var cancellationTokenSource = new CancellationTokenSource(nextFutureTask.QueuedAt!.Value - DateTime.UtcNow);
            await channel.Reader.WaitToReadAsync(cancellationTokenSource.Token);
        }

        return await WaitForTaskAsync(cancellationToken);
    }
}
