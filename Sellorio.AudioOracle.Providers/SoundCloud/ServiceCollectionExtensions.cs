using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.Common;
using SoundCloudExplode;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSoundCloudProvider(this IServiceCollection services)
    {
        services.AddCommonProviderServices();

        services.AddTransient<SoundCloudClient>();
        services.AddTransient<IDownloadSearchProvider, DownloadSearchProvider>();
        services.AddTransient<IDownloadProvider, DownloadProvider>();

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(SoundCloud)]);

        return services;
    }
}
