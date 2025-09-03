using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.Models;

namespace Sellorio.AudioOracle.Providers;

public interface IDownloadProvider : IProvider
{
    Task<Result> DownloadTrackAsync(ResolvedIds trackIds, string outputFilename);
    Task<ValueResult<ResolvedIds>> ResolveIdsFromTrackUrlAsync(string downloadUrl);
}
