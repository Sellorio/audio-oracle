using Sellorio.AudioOracle.Library.ApiTools;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;
internal interface IApiService
{
    Task<JsonNavigator> PostWithContextAsync(string url, object requestParameters);
}