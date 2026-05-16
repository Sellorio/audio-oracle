using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sellorio.AudioOracle.ServiceInterfaces.Events;
using Sellorio.AudioOracle.Services.Events;

namespace Sellorio.AudioOracle.Web.Events;

internal sealed class SignalREventService(IHubContext<AudioOracleEventsHub> hubContext, EventSubscriptionStore eventSubscriptionStore, IServiceProvider serviceProvider) : IEventService
{
    public async Task SendEvent<TEvents, TPayload>(string eventName, TPayload payload)
        where TEvents : class
    {
        var eventContract = EventContractRegistry.GetEventContract(typeof(TEvents), eventName);

        if (!eventContract.PayloadType.IsAssignableFrom(typeof(TPayload)))
        {
            throw new InvalidOperationException($"'{eventName}' expects payloads assignable to '{eventContract.PayloadType.FullName}', but received '{typeof(TPayload).FullName}'.");
        }

        await TriggerServerSideHandlersAsync<TEvents, TPayload>(eventName, payload);
        await TriggerClientSideHandlersAsync<TEvents, TPayload>(eventName, payload);
    }

    private async Task TriggerServerSideHandlersAsync<TEvents, TPayload>(string eventName, TPayload payload)
        where TEvents : class
    {
        var eventsInstance = serviceProvider.GetService(typeof(TEvents));

        if (eventsInstance != null)
        {
            try
            {
                var field = eventsInstance.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (field != null && field.GetValue(eventsInstance) is Delegate @delegate)
                {
                    var invocationList = @delegate.GetInvocationList();
                    var tasks = new List<Task>();

                    foreach (var d in invocationList)
                    {
                        tasks.Add((Task)d.DynamicInvoke(payload)!);
                    }

                    try
                    {
                        await Task.WhenAll(tasks.ToArray());
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }
    }

    private async Task TriggerClientSideHandlersAsync<TEvents, TPayload>(string eventName, TPayload payload)
        where TEvents : class
    {
        var subscription = EventSubscriptionKey.Create<TEvents>(eventName);

        if (!eventSubscriptionStore.HasSubscribers(subscription))
        {
            return;
        }

        await hubContext.Clients.Group(subscription.GroupName).SendAsync("Event", subscription.ServiceName, subscription.EventName, payload);
    }
}
