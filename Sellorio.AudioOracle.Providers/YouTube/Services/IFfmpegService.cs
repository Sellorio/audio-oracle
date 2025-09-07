using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal interface IFfmpegService
{
    Task<Result> ConvertToMp3Async(string source, string destination, int outputBitrateKbps, bool loudnessNormalization);
}
