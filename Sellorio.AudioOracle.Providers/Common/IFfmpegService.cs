using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Providers.Common;

public interface IFfmpegService
{
    Task<Result> ConvertToMp3Async(string source, string destination, int outputBitrateKbps, bool loudnessNormalization);
    Task EnsureFfmpegExecutableAsync();
}
