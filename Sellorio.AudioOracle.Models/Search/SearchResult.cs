using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models.Search;

public class SearchResult
{
    public required SearchResultType Type { get; init; }
    public required string Source { get; init; }

    public required string AlbumUrlId { get; init; }
    public required string AlbumId { get; init; }

    public required string Title { get; init; }
    public required string? AlternateTitle { get; init; }
    public required string AlbumTitle { get; init; }
    public required string? AlternateAlbumTitle { get; init; }
    public required string? AlbumArtUrl { get; init; }
    public required IList<string>? ArtistNames { get; init; }

    public SearchResultStatus Status { get; set; }
}
