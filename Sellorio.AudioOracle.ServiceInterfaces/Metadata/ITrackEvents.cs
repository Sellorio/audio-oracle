using Sellorio.AudioOracle.Models.Metadata;
using System;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata;

public interface ITrackEvents
{
    public event Func<Track, Task> TrackUpdated;
}
