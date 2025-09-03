using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class MediumDto
{
    public required Guid Id { get; init; }
    public required int Position { get; init; }
    public required string Format { get; init; }
    public required int TrackCount { get; init; }
    public required int TrackOffset { get; set; }
    public required IList<TrackDto>? Track { get; init; }
    public required IList<TrackDto>? Tracks { get; init; }
}
