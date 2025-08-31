using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class AlbumMetadata
{
    public string AlbumArtUrl { get; set; }
    public string Title { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public ushort? ReleaseYear { get; set; }
    public ushort TrackCount { get; set; }
    public IList<string> ArtistSourceIds { get; set; }
}
