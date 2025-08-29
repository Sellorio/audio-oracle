using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Services.Sessions;

namespace Sellorio.AudioOracle.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleServerSideServices(this IServiceCollection services)
    {
        return services
            .AddScoped<SessionState>()
            .AddScoped<ISessionService, SessionService>()
            ;
    }
}
