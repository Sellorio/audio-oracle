using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Providers.MusicBrainz;
using Sellorio.AudioOracle.Providers.SoundCloud;
using Sellorio.AudioOracle.Providers.YouTube;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.Metadata;
using Sellorio.AudioOracle.Services.Search;
using Sellorio.AudioOracle.Services.Sessions;

namespace Sellorio.AudioOracle.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleServerSideServices(this IServiceCollection services)
    {
        services
            .AddYouTubeProvider()
            .AddSoundCloudProvider()
            .AddMusicBrainzProvider()

            .AddScoped<SessionState>()
            .AddScoped<IProviderInvocationService, ProviderInvocationService>()

            // Content
            .AddScoped<IContentMapper, ContentMapper>()
            .AddScoped<IFileService, FileService>()

            // Metadata
            .AddScoped<IMetadataMapper, MetadataMapper>()
            .AddScoped<IAlbumService, AlbumService>()
            .AddScoped<IArtistCreationService, ArtistCreationService>()
            .AddScoped<IAlbumCreationService, AlbumCreationService>()

            // Search
            .AddScoped<ISearchService, SearchService>()

            // Sessions
            .AddScoped<ISessionService, SessionService>()
            ;

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(IAuthenticationService).Assembly, typeof(ServiceCollectionExtensions).Assembly]);

        return services;
    }
}
