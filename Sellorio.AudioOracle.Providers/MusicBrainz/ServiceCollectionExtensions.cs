using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Providers.MusicBrainz.Services;
using System;

namespace Sellorio.AudioOracle.Providers.MusicBrainz;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMusicBrainzProvider(this IServiceCollection services)
    {
        services
            .AddHttpClient(Constants.ProviderName + "MB", o =>
            {
                o.BaseAddress = new Uri("https://musicbrainz.org/ws/2/");
                o.DefaultRequestHeaders.UserAgent.ParseAdd(ProviderConstants.UserAgent);
            })
            .AddTypedClient<ISearchProvider, SearchProvider>()
            .AddTypedClient<IAlbumMetadataProvider, AlbumMetadataProvider>()
            .AddTypedClient<IMusicBrainzAlbumMetadataProvider, AlbumMetadataProvider>()
            .AddTypedClient<ITrackMetadataProvider, TrackMetadataProvider>()
            .AddTypedClient<IArtistMetadataProvider, ArtistMetadataProvider>();

        services
            .AddHttpClient(Constants.ProviderName + "CAA", o =>
            {
                o.BaseAddress = new Uri("https://coverartarchive.org/");
                o.DefaultRequestHeaders.UserAgent.ParseAdd(ProviderConstants.UserAgent);
            })
            .AddTypedClient<ICoverArtArchiveService, CoverArtArchiveService>();

        return services;
    }
}
