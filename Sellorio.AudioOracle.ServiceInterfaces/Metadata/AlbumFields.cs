using System;

namespace Sellorio.AudioOracle.ServiceInterfaces.Metadata
{
    [Flags]
    public enum AlbumFields
    {
        None = 0,
        Tracks = 1,
        Artists = 2
    }
}
