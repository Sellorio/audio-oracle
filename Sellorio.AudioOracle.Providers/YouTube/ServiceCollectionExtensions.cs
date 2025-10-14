using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.YouTube.Framework;
using Sellorio.AudioOracle.Providers.YouTube.Services;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Sellorio.AudioOracle.Providers.YouTube;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYouTubeProvider(this IServiceCollection services)
    {
        var cookieProvider = new CachedCookieProvider(Constants.CookiesPath);

        services.AddCommonProviderServices();

        services
            .AddHttpClient(Constants.ProviderName + "Api", o =>
            {
                o.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0");
                o.BaseAddress = new Uri("https://music.youtube.com/youtubei/v1/");

                //o.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
                //o.DefaultRequestHeaders.Add("Alt-Used", "music.youtube.com");
                //o.DefaultRequestHeaders.Referrer = new Uri("https://music.youtube.com/");
                //o.DefaultRequestHeaders.Add("X-Origin", "https://music.youtube.com/");
                //o.DefaultRequestHeaders.Add("X-Youtube-Client-Name", "67");
                //o.DefaultRequestHeaders.Add("X-Youtube-Client-Version", "1.20251006.03.00");
                //o.DefaultRequestHeaders.Add("X-Goog-AuthUser", "0");
                //o.DefaultRequestHeaders.Add("X-Goog-Visitor-Id", "Cgt3TTQwaDYyeV9zbyiJh6HHBjIKCgJBVRIEGgAgXA%3D%3D");
            })
            .ConfigurePrimaryHttpMessageHandler(CreateHandlerFromCookiesFile)
            .AddTypedClient<IApiService, ApiService>()
            .AddTypedClient<IPageService, PageService>()
            ;

        //services
        //    .AddHttpClient<IPageService, PageService>(o =>
        //    {
        //        o.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:143.0) Gecko/20100101 Firefox/143.0");
        //        o.BaseAddress = new Uri("https://music.youtube.com/");
        //    });

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

        return new ReadOnlyCookiesHttpClientHandler
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.All
        };
    }
}
