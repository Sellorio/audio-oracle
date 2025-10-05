using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.MusicBrainz.Services;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMusicBrainzProvider(this IServiceCollection services)
    {
        services.AddCommonProviderServices();
        services.AddTransient<ITrackMetadataProvider, TrackMetadataProvider>();

        services
            .AddHttpClient(Constants.ProviderName + "MB", o =>
            {
                o.BaseAddress = new Uri("https://musicbrainz.org/ws/2/");
                o.DefaultRequestHeaders.UserAgent.ParseAdd(ProviderConstants.UserAgent);
            })
            .AddTypedClient<IMetadataSearchProvider, MetadataSearchProvider>()
            .AddTypedClient<IAlbumMetadataProvider, AlbumMetadataProvider>()
            .AddTypedClient<IMusicBrainzAlbumMetadataProvider, AlbumMetadataProvider>()
            .AddTypedClient<IArtistMetadataProvider, ArtistMetadataProvider>();

        services
            .AddHttpClient(Constants.ProviderName + "CAA", o =>
            {
                o.BaseAddress = new Uri("https://coverartarchive.org/");
                o.DefaultRequestHeaders.UserAgent.ParseAdd(ProviderConstants.UserAgent);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true })
            .AddTypedClient<ICoverArtArchiveService, CoverArtArchiveService>();

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ServiceCollectionExtensions).Assembly],
            namespacesToInclude: [nameof(MusicBrainz)]);

        return services;
    }
}
