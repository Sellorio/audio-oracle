using Sellorio.AudioOracle.Client.Events;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Client.Metadata;

internal class TrackEvents(ISignalREventClient signalREventClient) : SignalREventServiceBase<ITrackEvents>(signalREventClient), ITrackEvents
{
    public event Func<Track, Task> TrackUpdated
    {
        add => AddHandler(nameof(TrackUpdated), value);
        remove => RemoveHandler(nameof(TrackUpdated), value);
    }
}
