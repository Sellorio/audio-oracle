namespace Sellorio.AudioOracle.Providers.Models;

public class DownloadSearchCriteria
{
    public required string TrackTitle { get; init; }
    public required string AlbumTitle { get; init; }
    public required string? MainArtist { get; init; }
}
