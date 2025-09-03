using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYouTubeProvider(this IServiceCollection services)
    {
        services
            .AddHttpClient(Constants.ProviderName + "Api", o =>
            {
                o.DefaultRequestHeaders.UserAgent.ParseAdd(ProviderConstants.UserAgent);
                o.BaseAddress = new System.Uri("https://music.youtube.com/youtubei/v1/");
            })
            .AddTypedClient<IApiService, ApiService>();

#error cookies integration


        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(YouTube)]);

        return services;
    }
}
