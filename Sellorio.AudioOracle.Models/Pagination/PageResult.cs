using System.Collections.Generic;

namespace Sellorio.AudioOracle.Models;

public class PageResult<TItem>
{
    public required IList<TItem> Items { get; init; }
    public required int TotalCount { get; init; }
}
