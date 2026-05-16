using System;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Services.Metadata;

internal sealed class AlbumEvents : IAlbumEvents
{
    public event Func<Album, Task>? AlbumCreated;

    public event Func<Album, Task>? AlbumUpdated;

    public event Func<Album, Task>? AlbumDeleted;
}
