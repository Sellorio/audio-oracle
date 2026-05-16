using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Events;

public static class EventContractRegistry
{
    private static readonly Lazy<IReadOnlyDictionary<Type, EventServiceContract>> _contractsByType = new(CreateContractsByType);
    private static readonly Lazy<IReadOnlyDictionary<string, EventServiceContract>> _contractsByName = new(() => _contractsByType.Value.Values.ToDictionary(x => x.ServiceName, StringComparer.Ordinal));

    public static IReadOnlyCollection<Type> GetEventInterfaceTypes()
    {
        return _contractsByType.Value.Keys.ToArray();
    }

    public static string GetServiceName(Type interfaceType)
    {
        return GetServiceContract(interfaceType).ServiceName;
    }

    public static EventServiceContract GetServiceContract(Type interfaceType)
    {
        if (!_contractsByType.Value.TryGetValue(interfaceType, out var contract))
        {
            throw new InvalidOperationException($"'{interfaceType.FullName ?? interfaceType.Name}' is not a supported event service interface.");
        }

        return contract;
    }

    public static EventContract GetEventContract(Type interfaceType, string eventName)
    {
        var serviceContract = GetServiceContract(interfaceType);

        if (!serviceContract.Events.TryGetValue(eventName, out var eventContract))
        {
            throw new InvalidOperationException($"'{eventName}' is not a supported event on '{serviceContract.ServiceName}'.");
        }

        return eventContract;
    }

    public static EventContract GetEventContract(string serviceName, string eventName)
    {
        if (!_contractsByName.Value.TryGetValue(serviceName, out var serviceContract))
        {
            throw new InvalidOperationException($"'{serviceName}' is not a supported event service interface.");
        }

        if (!serviceContract.Events.TryGetValue(eventName, out var eventContract))
        {
            throw new InvalidOperationException($"'{eventName}' is not a supported event on '{serviceName}'.");
        }

        return eventContract;
    }

    private static IReadOnlyDictionary<Type, EventServiceContract> CreateContractsByType()
    {
        return typeof(EventContractRegistry)
            .Assembly
            .GetTypes()
            .Where(x => x.IsInterface)
            .Where(x => x.Name.EndsWith("Events", StringComparison.Ordinal))
            .Select(CreateServiceContract)
            .ToDictionary(x => x.InterfaceType);
    }

    private static EventServiceContract CreateServiceContract(Type interfaceType)
    {
        var serviceName = interfaceType.FullName ?? interfaceType.Name;
        var events = interfaceType.GetEvents().Select(CreateEventContract).ToDictionary(x => x.EventName, StringComparer.Ordinal);
        return new EventServiceContract(interfaceType, serviceName, events);
    }

    private static EventContract CreateEventContract(EventInfo eventInfo)
    {
        var eventHandlerType = eventInfo.EventHandlerType ?? throw new InvalidOperationException($"'{eventInfo.Name}' does not declare an event handler type.");
        var invokeMethod = eventHandlerType.GetMethod(nameof(Action.Invoke)) ?? throw new InvalidOperationException($"'{eventInfo.Name}' does not declare an invokable event handler type.");
        var parameters = invokeMethod.GetParameters();

        if (parameters.Length != 1)
        {
            throw new InvalidOperationException($"'{eventInfo.Name}' must use a delegate with a single payload parameter.");
        }

        if (invokeMethod.ReturnType != typeof(Task))
        {
            throw new InvalidOperationException($"'{eventInfo.Name}' must use a delegate that returns Task.");
        }

        return new EventContract(eventInfo.Name, parameters[0].ParameterType);
    }
}

public sealed record EventServiceContract(Type InterfaceType, string ServiceName, IReadOnlyDictionary<string, EventContract> Events);

public sealed record EventContract(string EventName, Type PayloadType);
