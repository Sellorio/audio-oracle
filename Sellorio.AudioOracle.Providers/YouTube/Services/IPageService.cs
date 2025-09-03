using Sellorio.AudioOracle.Library.ApiTools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;
internal interface IPageService
{
    Task<IList<JsonNavigator>> GetPageInitialDataAsync(string url);
}