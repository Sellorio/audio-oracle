using System;
using System.Collections.Generic;
using System.IO;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Album
{
    public const int SourceUrlIdMaxLength = 400;
    public const int SourceIdMaxLength = 50;
    public const int TitleMaxLength = 300;

    public int Id { get; set; }
    public FileInfo AlbumArt { get; set; }
    public IList<Track> Tracks { get; set; }

    public string Title { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public ushort? ReleaseYear { get; set; }
    public ushort TrackCount { get; set; }
    public IList<Artist> Artists { get; set; }
}
