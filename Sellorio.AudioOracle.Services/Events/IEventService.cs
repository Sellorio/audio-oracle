using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Events;

public interface IEventService
{
    Task SendEvent<TEvents, TPayload>(string eventName, TPayload payload)
        where TEvents : class;
}
