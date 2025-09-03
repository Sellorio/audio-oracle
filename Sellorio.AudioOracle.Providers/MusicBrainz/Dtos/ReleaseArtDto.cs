using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleaseArtDto
{
    public required string Release { get; init; }
    public required IList<ReleaseArtImageDto> Images { get; init; }
}
