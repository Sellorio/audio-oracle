using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.ServiceInterfaces.Providers;

public interface IProviderCatalogService
{
    Task<ValueResult<IList<ProviderInfo>>> GetProvidersInfoAsync();
}
