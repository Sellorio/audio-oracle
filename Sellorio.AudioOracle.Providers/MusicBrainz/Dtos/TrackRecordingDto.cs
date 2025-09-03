using System;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class TrackRecordingDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
}
