using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Metadata;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers;

public interface IDownloadProvider : IProvider
{
    Task<Result> DownloadTrackAsync(Track track, string outputFilename);
}
