namespace Sellorio.AudioOracle.Providers.Models;

public class DownloadSearchResult
{
    public required string Source { get; init; }
    public required ResolvedIds Ids { get; init; }

    public required string Title { get; init; }
    public required string? AlternateTitle { get; init; }
    public required string? AlbumArtUrl { get; init; }
}
