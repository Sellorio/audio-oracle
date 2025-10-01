using System.Collections.Generic;
using Sellorio.AudioOracle.Models.Metadata;

namespace Sellorio.AudioOracle.Models.Search;

public class DownloadSearchResult
{
    public required DownloadSource DownloadSource { get; init; }
    public required string Title { get; init; }
    public required string AlbumTitle { get; init; }
    public required string? AlbumArtUrl { get; init; }
    public required IList<string>? ArtistNames { get; init; }
}
