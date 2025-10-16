using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Providers;
using Sellorio.AudioOracle.Providers;
using Sellorio.AudioOracle.ServiceInterfaces.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Services.Providers;

internal class ProviderCatalogService(IEnumerable<IMetadataSearchProvider> metadataSearchProviders, IEnumerable<IDownloadProvider> downloadProviders) : IProviderCatalogService
{
    public Task<ValueResult<IList<ProviderInfo>>> GetProvidersInfoAsync()
    {
        var providerNames =
            Enumerable.Concat(
                metadataSearchProviders.Select(x => x.ProviderName),
                downloadProviders.Select(x => x.ProviderName))
            .Distinct().ToList();

        var result =
            providerNames
                .Select(x => new ProviderInfo
                {
                    Name = x,
                    IsMetadataSource = metadataSearchProviders.Any(y => y.ProviderName == x),
                    IsDownloadSource = downloadProviders.Any(y => y.ProviderName == x)
                })
                .ToArray();

        return Task.FromResult(ValueResult.Success<IList<ProviderInfo>>(result));
    }
}
