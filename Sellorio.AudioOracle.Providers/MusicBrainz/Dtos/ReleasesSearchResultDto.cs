using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleasesSearchResultDto
{
    public required int Count { get; init; }
    public required int Offset { get; init; }
    public required IList<ReleaseDto> Releases { get; init; }
}
