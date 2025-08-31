using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Search;

public interface ISearchService
{
    Task<ValueResult<IList<SearchResult>>> SearchAsync(string searchText);
}
