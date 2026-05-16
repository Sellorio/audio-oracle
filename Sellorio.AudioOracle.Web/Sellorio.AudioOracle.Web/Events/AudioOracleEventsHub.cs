using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Web.Events;

internal class AudioOracleEventsHub(ISessionService sessionService, EventSubscriptionStore eventSubscriptionStore) : Hub
{
    public async Task SubscribeAsync(string serviceName, string eventName, string? sessionToken)
    {
        await ValidateSessionAsync(sessionToken);

        var subscription = EventSubscriptionKey.Create(serviceName, eventName);

        if (!eventSubscriptionStore.AddSubscription(Context.ConnectionId, subscription))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, subscription.GroupName);
    }

    public async Task UnsubscribeAsync(string serviceName, string eventName)
    {
        var subscription = EventSubscriptionKey.Create(serviceName, eventName);

        if (!eventSubscriptionStore.RemoveSubscription(Context.ConnectionId, subscription))
        {
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, subscription.GroupName);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var subscription in eventSubscriptionStore.RemoveConnection(Context.ConnectionId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, subscription.GroupName);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task ValidateSessionAsync(string? sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            throw new HubException("Unauthorized.");
        }

        var result = await sessionService.UseSessionAsync(sessionToken);

        if (!result.WasSuccess)
        {
            throw new HubException("Unauthorized.");
        }
    }
}
