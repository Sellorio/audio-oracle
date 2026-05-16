using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.ServiceInterfaces.Events;

namespace Sellorio.AudioOracle.Client.Events;

internal sealed class SignalREventClient(string baseAddress, IAudioOracleSessionTokenProvider sessionTokenProvider, ILogger<SignalREventClient> logger) : ISignalREventClient, IAsyncDisposable
{
    private const string HubMethodName = "Event";
    private const string SubscribeMethodName = "SubscribeAsync";
    private const string UnsubscribeMethodName = "UnsubscribeAsync";
    private readonly object _lock = new();
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly Dictionary<EventSubscriptionKey, List<Delegate>> _handlers = [];
    private HubConnection? _connection;

    public void AddHandler(Type interfaceType, string eventName, Delegate handler)
    {
        var subscription = EventSubscriptionKey.Create(interfaceType, eventName);
        var shouldSubscribe = false;

        lock (_lock)
        {
            if (!_handlers.TryGetValue(subscription, out var handlers))
            {
                handlers = [];
                _handlers.Add(subscription, handlers);
            }

            shouldSubscribe = handlers.Count == 0;
            handlers.Add(handler);
        }

        if (shouldSubscribe)
        {
            _ = SynchronizeSubscriptionAsync(subscription, true);
        }
    }

    public void RemoveHandler(Type interfaceType, string eventName, Delegate handler)
    {
        var subscription = EventSubscriptionKey.Create(interfaceType, eventName);
        var shouldUnsubscribe = false;

        lock (_lock)
        {
            if (!_handlers.TryGetValue(subscription, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                _handlers.Remove(subscription);
                shouldUnsubscribe = true;
            }
        }

        if (shouldUnsubscribe)
        {
            _ = SynchronizeSubscriptionAsync(subscription, false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }

    private async Task SynchronizeSubscriptionAsync(EventSubscriptionKey subscription, bool subscribe)
    {
        try
        {
            var connection = await GetConnectionAsync();

            if (subscribe)
            {
                var sessionToken = await sessionTokenProvider.GetSessionTokenAsync();

                if (string.IsNullOrWhiteSpace(sessionToken))
                {
                    return;
                }

                await connection.InvokeAsync(SubscribeMethodName, subscription.ServiceName, subscription.EventName, sessionToken);
            }
            else if (connection.State == HubConnectionState.Connected)
            {
                await connection.InvokeAsync(UnsubscribeMethodName, subscription.ServiceName, subscription.EventName);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to synchronize SignalR event subscription for {ServiceName}.{EventName}.", subscription.ServiceName, subscription.EventName);
        }
    }

    private async Task<HubConnection> GetConnectionAsync()
    {
        await _connectionLock.WaitAsync();

        try
        {
            if (_connection == null)
            {
                _connection =
                    new HubConnectionBuilder()
                        .WithUrl(new Uri(new Uri(baseAddress), "hubs/events"))
                        .WithAutomaticReconnect()
                        .AddJsonProtocol(o =>
                        {
                            foreach (var converter in RestClient.JsonOptions.Converters)
                            {
                                o.PayloadSerializerOptions.Converters.Add(converter);
                            }
                        })
                        .Build();

                _connection.On<string, string, JsonElement>(HubMethodName, HandleEventAsync);
                _connection.Reconnected += OnReconnectedAsync;
            }

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }

            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private Task OnReconnectedAsync(string? _)
    {
        return ResubscribeAsync();
    }

    private async Task ResubscribeAsync()
    {
        var sessionToken = await sessionTokenProvider.GetSessionTokenAsync();

        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            return;
        }

        var connection = await GetConnectionAsync();
        EventSubscriptionKey[] subscriptions;

        lock (_lock)
        {
            subscriptions = _handlers.Keys.ToArray();
        }

        foreach (var subscription in subscriptions)
        {
            await connection.InvokeAsync(SubscribeMethodName, subscription.ServiceName, subscription.EventName, sessionToken);
        }
    }

    private async Task HandleEventAsync(string serviceName, string eventName, JsonElement payload)
    {
        var eventContract = EventContractRegistry.GetEventContract(serviceName, eventName);
        var payloadModel = payload.Deserialize(eventContract.PayloadType, RestClient.JsonOptions);

        Delegate[] handlers;
        lock (_lock)
        {
            if (!_handlers.TryGetValue(EventSubscriptionKey.Create(serviceName, eventName), out var eventHandlers) || eventHandlers.Count == 0)
            {
                return;
            }

            handlers = eventHandlers.ToArray();
        }

        if (handlers.Length == 1)
        {
            await InvokeHandlerAsync(handlers[0], payloadModel);
            return;
        }

        await Task.WhenAll(handlers.Select(x => InvokeHandlerAsync(x, payloadModel)));
    }

    private static Task InvokeHandlerAsync(Delegate handler, object? payload)
    {
        try
        {
            return (Task)handler.DynamicInvoke(payload)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            return Task.FromException(ex.InnerException);
        }
    }
}
