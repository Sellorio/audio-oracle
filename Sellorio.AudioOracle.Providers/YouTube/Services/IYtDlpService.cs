using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal interface IYtDlpService
{
    Task<Result> InvokeYtDlpAsync(string arguments, string outputDirectory = "");
}
