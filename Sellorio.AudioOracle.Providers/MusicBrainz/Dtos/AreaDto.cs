using System;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class AreaDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
