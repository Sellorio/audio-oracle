using System.Collections.Generic;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.Models.Search;

namespace Sellorio.AudioOracle.ServiceInterfaces.Search;

public interface ISearchService
{
    Task<ValueResult<IList<SearchResult>>> SearchAsync(MetadataSearchPost search);
    Task<ValueResult<IList<DownloadSearchResult>>> SearchForDownloadAsync(int trackId);
    Task<ValueResult<DownloadSource>> SearchForDownloadByUrlAsync(string url);
}
