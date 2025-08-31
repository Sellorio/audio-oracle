using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class TrackMetadata
{
    public ResolvedIds DownloadIds { get; set; }
    public string Title { get; set; }
    public string AlternateTitle { get; set; }
    public int TrackNumber { get; set; }
    public IList<string> ArtistSourceIds { get; set; }
}
