using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class DownloadSearchResult
{
    public required string Source { get; init; }
    public required ResolvedIds Ids { get; init; }
    public required string Title { get; init; }
    public required string AlbumTitle { get; init; }
    public required string? AlbumArtUrl { get; init; }
    public required IList<string>? ArtistNames { get; init; }
}
