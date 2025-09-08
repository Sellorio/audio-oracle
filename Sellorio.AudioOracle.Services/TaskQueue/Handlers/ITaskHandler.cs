using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.TaskQueue.Handlers;

public interface ITaskHandler
{
    string HandlerName => GetType().Name;

    Task HandleAsync(TaskHandlerContext context);
}
