using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface ISearchProvider : IProvider
{
    Task<ValueResult<PagedList<SearchResult>>> SearchAsync(string searchText, int pageSize);
}
