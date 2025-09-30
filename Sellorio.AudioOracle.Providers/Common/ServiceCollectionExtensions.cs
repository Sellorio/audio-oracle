using Microsoft.Extensions.DependencyInjection;

namespace Sellorio.AudioOracle.Providers.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonProviderServices(this IServiceCollection services)
    {
        services.AddTransient<IFfmpegService, FfmpegService>();
        return services;
    }
}
