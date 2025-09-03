using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.Models;

public class TrackMetadata
{
    public required string? AlbumArtOverrideUrl { get; init; }
    public required ResolvedIds? DownloadIds { get; init; }
    public required string Title { get; init; }
    public required string? AlternateTitle { get; init; }
    public required TimeSpan? Duration { get; init; }
    public required int TrackNumber { get; init; }
    public required IList<ResolvedIds> ArtistIds { get; init; }
}
