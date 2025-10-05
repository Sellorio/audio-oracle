using System.Collections.Generic;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Models;

internal class SearchResultDto<TItem>
{
    public required IList<TItem> Collection { get; set; }
    public required string? NextHref { get; set; }
}
