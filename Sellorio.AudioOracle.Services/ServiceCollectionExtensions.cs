using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.Services.Metadata;
using Sellorio.AudioOracle.Services.Sessions;

namespace Sellorio.AudioOracle.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleServerSideServices(this IServiceCollection services)
    {
        return services
            .AddScoped<SessionState>()

            // Metadata
            .AddScoped<IMetadataMapper, MetadataMapper>()
            .AddScoped<IAlbumService, AlbumService>()

            // Sessions
            .AddScoped<ISessionService, SessionService>()
            ;
    }
}
