using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models.Metadata;

public class Track
{
    public required int Id { get; init; }
    public required int AlbumId { get; init; }
    public required bool IsRequested { get; init; }
    public required TrackStatus Status { get; init; }
    public required string StatusText { get; init; }
    public required string Filename { get; init; }

    public required string Title { get; init; }
    public required string AlternateTitle { get; init; }
    public required TimeSpan? Duration { get; init; }
    public required int? TrackNumber { get; init; }
    public required IList<Artist> Artists { get; init; }
}
