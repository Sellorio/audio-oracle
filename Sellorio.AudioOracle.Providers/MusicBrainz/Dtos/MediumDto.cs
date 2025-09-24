using System;
using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class MediumDto
{
    public required Guid Id { get; init; }
    public int? Position { get; init; }
    public string? Format { get; init; }
    public required int TrackCount { get; init; }
    public int? TrackOffset { get; set; }
    public IList<TrackDto>? Track { get; init; }
    public IList<TrackDto>? Tracks { get; init; }
}
