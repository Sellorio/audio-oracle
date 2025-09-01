using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.MusicBrainz.Dtos;

internal class ReleaseArtDto
{
    public string Release { get; set; }
    public IList<ReleaseArtImageDto> Images { get; set; }
}
