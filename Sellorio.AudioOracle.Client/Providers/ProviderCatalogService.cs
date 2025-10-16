using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Providers;
using Sellorio.AudioOracle.ServiceInterfaces.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Client.Providers;

internal class ProviderCatalogService(IRestClient restClient) : IProviderCatalogService
{
    public async Task<ValueResult<IList<ProviderInfo>>> GetProvidersInfoAsync()
    {
        return await restClient.Get($"/providers").ToValueResult<IList<ProviderInfo>>();
    }
}
