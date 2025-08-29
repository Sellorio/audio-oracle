using System.Collections.Generic;

namespace Sellorio.AudioOracle.Library.Results
{
    public class PagedListWithCount<TItem>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<TItem> Items { get; set; }
    }
}
