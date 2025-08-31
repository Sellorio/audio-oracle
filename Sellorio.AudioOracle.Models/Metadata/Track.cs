using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Track
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public bool IsRequested { get; set; }
    public TrackStatus Status { get; set; }
    public string StatusText { get; set; }
    public string Filename { get; set; }

    public string Title { get; set; }
    public string AlternateTitle { get; set; }
    public int? TrackNumber { get; set; }
    public IList<Artist> Artists { get; set; }
}
