using System.Collections.Generic;
using Sellorio.AudioOracle.Models.Search;

namespace Sellorio.AudioOracle.Providers.Models;

public class MetadataSearchResult
{
    public required SearchResultType Type { get; init; }
    public required string Source { get; init; }

    public required ResolvedIds AlbumIds { get; init; }

    public required string Title { get; init; }
    public required string? AlternateTitle { get; init; }
    public required string AlbumTitle { get; init; }
    public required string? AlternateAlbumTitle { get; init; }
    public required string? AlbumArtUrl { get; init; }
    public required IList<string>? ArtistNames { get; init; }
}
