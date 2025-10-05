using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.SoundCloud.Services;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSoundCloudProvider(this IServiceCollection services)
    {
        services.AddCommonProviderServices();

        services.AddSingleton<ISoundCloudClientIdService, SoundCloudClientIdService>();

        services.AddHttpClient<ISoundCloudApiService, SoundCloudApiService>(o =>
        {
            o.BaseAddress = new System.Uri("https://api-v2.soundcloud.com/");
        });

        services.AddTransient<IDownloadSearchProvider, DownloadSearchProvider>();
        services.AddTransient<IDownloadProvider, DownloadProvider>();

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(SoundCloud)]);

        return services;
    }
}
