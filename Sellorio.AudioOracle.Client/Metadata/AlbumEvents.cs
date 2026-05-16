using System;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Client.Events;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Client.Metadata;

internal sealed class AlbumEvents(ISignalREventClient signalREventClient) : SignalREventServiceBase<IAlbumEvents>(signalREventClient), IAlbumEvents
{
    public event Func<Album, Task> AlbumCreated
    {
        add => AddHandler(nameof(AlbumCreated), value);
        remove => RemoveHandler(nameof(AlbumCreated), value);
    }

    public event Func<Album, Task> AlbumUpdated
    {
        add => AddHandler(nameof(AlbumUpdated), value);
        remove => RemoveHandler(nameof(AlbumUpdated), value);
    }

    public event Func<Album, Task> AlbumDeleted
    {
        add => AddHandler(nameof(AlbumDeleted), value);
        remove => RemoveHandler(nameof(AlbumDeleted), value);
    }
}
