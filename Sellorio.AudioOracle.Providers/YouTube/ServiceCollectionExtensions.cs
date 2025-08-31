using Microsoft.Extensions.DependencyInjection;

namespace Sellorio.AudioOracle.Providers.YouTube;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYouTubeProvider(this IServiceCollection services)
    {
        return services;
    }
}
