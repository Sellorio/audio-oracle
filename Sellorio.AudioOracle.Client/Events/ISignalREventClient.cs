using System;

namespace Sellorio.AudioOracle.Client.Events;

internal interface ISignalREventClient
{
    void AddHandler(Type interfaceType, string eventName, Delegate handler);
    void RemoveHandler(Type interfaceType, string eventName, Delegate handler);
}
