using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSoundCloudProvider(this IServiceCollection services)
    {

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(SoundCloud)]);

        return services;
    }
}
