using System;
using System.Collections.Generic;
using Sellorio.AudioOracle.Models.Content;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Album
{
    public const int SourceMaxLength = 100;
    public const int SourceUrlIdMaxLength = 400;
    public const int SourceIdMaxLength = 50;
    public const int TitleMaxLength = 300;

    public required int Id { get; init; }
    public required FileInfo AlbumArt { get; init; }
    public required IList<Track> Tracks { get; init; }

    public required string Title { get; init; }
    public required DateOnly? ReleaseDate { get; init; }
    public required ushort? ReleaseYear { get; init; }
    public required ushort TrackCount { get; init; }
    public required IList<Artist> Artists { get; init; }
}
