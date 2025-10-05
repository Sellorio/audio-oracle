using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Models;

internal class MediaDto
{
    public required IList<TranscodingDto> Transcodings { get; set; }
}
