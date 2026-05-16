using System;

namespace Sellorio.AudioOracle.Client.Events;

internal abstract class SignalREventServiceBase<TEvents>(ISignalREventClient signalREventClient)
    where TEvents : class
{
    protected void AddHandler(string eventName, Delegate handler)
    {
        signalREventClient.AddHandler(typeof(TEvents), eventName, handler);
    }

    protected void RemoveHandler(string eventName, Delegate handler)
    {
        signalREventClient.RemoveHandler(typeof(TEvents), eventName, handler);
    }
}
