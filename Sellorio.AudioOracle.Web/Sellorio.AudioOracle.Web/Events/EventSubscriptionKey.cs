using System;
using Sellorio.AudioOracle.ServiceInterfaces.Events;

namespace Sellorio.AudioOracle.Web.Events;

internal readonly record struct EventSubscriptionKey(string ServiceName, string EventName)
{
    public string GroupName => $"{ServiceName}:{EventName}";

    public static EventSubscriptionKey Create<TEvents>(string eventName)
        where TEvents : class
    {
        return Create(typeof(TEvents), eventName);
    }

    public static EventSubscriptionKey Create(Type interfaceType, string eventName)
    {
        EventContractRegistry.GetEventContract(interfaceType, eventName);
        return new EventSubscriptionKey(EventContractRegistry.GetServiceName(interfaceType), eventName);
    }

    public static EventSubscriptionKey Create(string serviceName, string eventName)
    {
        EventContractRegistry.GetEventContract(serviceName, eventName);
        return new EventSubscriptionKey(serviceName, eventName);
    }
}
