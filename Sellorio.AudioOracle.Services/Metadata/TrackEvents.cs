using System;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Services.Metadata;

internal sealed class TrackEvents : ITrackEvents
{
    public event Func<Track, Task>? TrackUpdated;
}
