using System;
using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.YouTube.Services;

namespace Sellorio.AudioOracle.Providers.YouTube;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYouTubeProvider(this IServiceCollection services)
    {
        services.AddCommonProviderServices();

        services
            .AddHttpClient(Constants.ProviderName + "Api", o =>
            {
                o.DefaultRequestHeaders.UserAgent.ParseAdd(ProviderConstants.UserAgent);
                o.BaseAddress = new Uri("https://music.youtube.com/youtubei/v1/");
            })
            .ConfigurePrimaryHttpMessageHandler(
                () => new HttpClientHandlerWithCookies<IYouTubeAlbumMetadataProvider>(Constants.CookiesPath))
            .AddTypedClient<IApiService, ApiService>();

        services
            .AddTransient<IBrowseService, BrowseService>()
            .AddTransient<ITrackIdsResolver, TrackIdsResolver>()
            .AddTransient<IMetadataSearchProvider, MetadataSearchProvider>()
            .AddTransient<IAlbumMetadataProvider, AlbumMetadataProvider>()
            .AddTransient<IYouTubeAlbumMetadataProvider, AlbumMetadataProvider>()
            .AddTransient<IArtistMetadataProvider, ArtistMetadataProvider>()
            .AddTransient<ITrackMetadataProvider, TrackMetadataProvider>()
            .AddTransient<IDownloadSearchProvider, DownloadSearchProvider>()
            .AddTransient<IDownloadProvider, DownloadProvider>()
            .AddTransient<IYtDlpService, YtDlpService>();

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(YouTube)]);

        return services;
    }
}
