using System.Collections.Generic;
using System.Linq;

namespace Sellorio.AudioOracle.Web.Events;

internal sealed class EventSubscriptionStore
{
    private readonly object _lock = new();
    private readonly Dictionary<string, HashSet<EventSubscriptionKey>> _subscriptionsByConnectionId = [];
    private readonly Dictionary<EventSubscriptionKey, HashSet<string>> _connectionIdsBySubscription = [];

    public bool AddSubscription(string connectionId, EventSubscriptionKey subscription)
    {
        lock (_lock)
        {
            if (!_subscriptionsByConnectionId.TryGetValue(connectionId, out var subscriptions))
            {
                subscriptions = [];
                _subscriptionsByConnectionId.Add(connectionId, subscriptions);
            }

            if (!subscriptions.Add(subscription))
            {
                return false;
            }

            if (!_connectionIdsBySubscription.TryGetValue(subscription, out var connectionIds))
            {
                connectionIds = [];
                _connectionIdsBySubscription.Add(subscription, connectionIds);
            }

            connectionIds.Add(connectionId);
            return true;
        }
    }

    public bool RemoveSubscription(string connectionId, EventSubscriptionKey subscription)
    {
        lock (_lock)
        {
            if (!_subscriptionsByConnectionId.TryGetValue(connectionId, out var subscriptions) || !subscriptions.Remove(subscription))
            {
                return false;
            }

            if (subscriptions.Count == 0)
            {
                _subscriptionsByConnectionId.Remove(connectionId);
            }

            if (_connectionIdsBySubscription.TryGetValue(subscription, out var connectionIds))
            {
                connectionIds.Remove(connectionId);

                if (connectionIds.Count == 0)
                {
                    _connectionIdsBySubscription.Remove(subscription);
                }
            }

            return true;
        }
    }

    public bool HasSubscribers(EventSubscriptionKey subscription)
    {
        lock (_lock)
        {
            return _connectionIdsBySubscription.TryGetValue(subscription, out var connectionIds) && connectionIds.Count != 0;
        }
    }

    public IReadOnlyList<EventSubscriptionKey> RemoveConnection(string connectionId)
    {
        lock (_lock)
        {
            if (!_subscriptionsByConnectionId.Remove(connectionId, out var subscriptions))
            {
                return [];
            }

            foreach (var subscription in subscriptions)
            {
                if (_connectionIdsBySubscription.TryGetValue(subscription, out var connectionIds))
                {
                    connectionIds.Remove(connectionId);

                    if (connectionIds.Count == 0)
                    {
                        _connectionIdsBySubscription.Remove(subscription);
                    }
                }
            }

            return subscriptions.ToArray();
        }
    }
}
