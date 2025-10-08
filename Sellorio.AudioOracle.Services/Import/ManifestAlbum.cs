using System.Collections.Generic;

namespace Sellorio.AudioOracle.Services.Import;

internal class ManifestAlbum
{
    public required string FolderName { get; set; }
    public required IList<ManifestTrack> Tracks { get; set; }
}
