using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Providers.MusicBrainz;
using Sellorio.AudioOracle.Providers.SoundCloud;
using Sellorio.AudioOracle.Providers.YouTube;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using Sellorio.AudioOracle.Services.Metadata;
using Sellorio.AudioOracle.Services.Search;
using Sellorio.AudioOracle.Services.Sessions;

namespace Sellorio.AudioOracle.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleServerSideServices(this IServiceCollection services)
    {
        return services
            .AddYouTubeProvider()
            .AddSoundCloudProvider()
            .AddMusicBrainzProvider()

            .AddScoped<SessionState>()

            // Metadata
            .AddScoped<IMetadataMapper, MetadataMapper>()
            .AddScoped<IAlbumService, AlbumService>()

            // Search
            .AddScoped<ISearchService, SearchService>()

            // Sessions
            .AddScoped<ISessionService, SessionService>()
            ;
    }
}
