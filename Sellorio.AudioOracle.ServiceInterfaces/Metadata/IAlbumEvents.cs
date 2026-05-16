using Sellorio.AudioOracle.Models.Metadata;
using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface IAlbumEvents
{
    public event Func<Album, Task> AlbumCreated;
    public event Func<Album, Task> AlbumUpdated;
    public event Func<Album, Task> AlbumDeleted;
}
