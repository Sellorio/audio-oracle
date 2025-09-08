using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Sellorio.AudioOracle.Services.TaskQueue;

namespace Sellorio.AudioOracle.Web.TaskQueue;

public class TaskQueueHostedService(ITaskQueueService taskQueueService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await taskQueueService.RunAsync(stoppingToken);
    }
}
