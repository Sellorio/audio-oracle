using System.Collections.Generic;

namespace Sellorio.AudioOracle.Library.Results
{
    public class PagedList<TItem>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<TItem> Items { get; set; }
    }
}
