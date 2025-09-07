using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class AlbumMetadata
{
    public required string? AlbumArtUrl { get; init; }
    public required string Title { get; init; }
    public required DateOnly? ReleaseDate { get; init; }
    public required ushort? ReleaseYear { get; init; }
    public required IList<ResolvedIds> ArtistIds { get; init; }
    public required IList<AlbumTrackMetadata> Tracks { get; init; }
}
