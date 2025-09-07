using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
                o.BaseAddress = new Uri("https://music.youtube.com/youtubei/v1/");
            })
            .ConfigurePrimaryHttpMessageHandler(CreateHandlerFromCookiesFile)
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
            .AddTransient<IYtDlpService, YtDlpService>()
            .AddTransient<IFfmpegService, FfmpegService>();

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(YouTube)]);

        return services;
    }

    private static HttpClientHandler CreateHandlerFromCookiesFile()
    {
        var cookieContainer = new CookieContainer();

        foreach (var line in File.ReadLines(Constants.CookiesPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue; // skip comments and empty lines

            // Format: domain \t flag \t path \t secure \t expiry \t name \t value
            var parts = line.Split('\t');
            if (parts.Length != 7)
                continue; // skip invalid lines

            string domain = parts[0];
            string path = parts[2];
            bool secure = parts[3].Equals("TRUE", StringComparison.OrdinalIgnoreCase);
            string name = parts[5];
            string value = parts[6];

            cookieContainer.Add(new Cookie(name, value, path, domain.TrimStart('.')));
        }

        return new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.All
        };
    }
}
