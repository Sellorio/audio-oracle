using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Client.Metadata;
using Sellorio.AudioOracle.Client.Search;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleClientSideServices<TSessionTokenProvider>(this IServiceCollection services)
        where TSessionTokenProvider : class, IAudioOracleSessionTokenProvider
    {
        const string clientName = "AORestHttpClient";

        services.AddScoped<IAudioOracleSessionTokenProvider, TSessionTokenProvider>();
        services.AddHttpClient(clientName, o => o.BaseAddress = new System.Uri("api"));

        // Metadata
        services.TryAddRestClient<IAlbumService, AlbumService>(clientName);
        services.TryAddRestClient<IAlbumCreationService, AlbumCreationService>(clientName);

        // Search
        services.TryAddRestClient<ISearchService, SearchService>(clientName);

        // Sessions
        services.TryAddRestClient<IAuthenticationService, AuthenticationService>(clientName);

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(services, [typeof(IAuthenticationService).Assembly]);

        return services;
    }
}
