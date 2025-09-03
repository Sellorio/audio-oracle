using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface IMetadataSearchProvider : IProvider
{
    Task<ValueResult<PagedList<MetadataSearchResult>>> SearchForMetadataAsync(string searchText, int pageSize);
}
