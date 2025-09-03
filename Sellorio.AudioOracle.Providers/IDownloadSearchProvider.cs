using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;

namespace Sellorio.AudioOracle.Providers;

public interface IDownloadSearchProvider : IProvider
{
    Task<ValueResult<PagedList<DownloadSearchResult>>> SearchForDownloadAsync(DownloadSearchCriteria searchCriteria, int pageSize);
}
