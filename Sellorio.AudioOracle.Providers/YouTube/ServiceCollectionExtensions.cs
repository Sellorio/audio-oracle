using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;

namespace Sellorio.AudioOracle.Providers.YouTube;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYouTubeProvider(this IServiceCollection services)
    {

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(YouTube)]);

        return services;
    }
}
