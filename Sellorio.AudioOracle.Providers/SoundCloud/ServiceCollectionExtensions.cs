using Microsoft.Extensions.DependencyInjection;

namespace Sellorio.AudioOracle.Providers.SoundCloud;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSoundCloudProvider(this IServiceCollection services)
    {
        return services;
    }
}
