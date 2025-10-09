using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Client.Content;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Client.Metadata;
using Sellorio.AudioOracle.Client.Search;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.ServiceInterfaces.Content;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleClientSideServices<TSessionTokenProvider>(this IServiceCollection services, string baseAddress)
        where TSessionTokenProvider : class, IAudioOracleSessionTokenProvider
    {
        const string clientName = "AORestHttpClient";

        services.AddSingleton<IAudioOracleSessionTokenProvider, TSessionTokenProvider>();
        services.AddHttpClient(clientName, o => o.BaseAddress = new System.Uri(baseAddress + "api/"));

        // Content
        services.TryAddRestClient<IDataFileService, DataFileService>(clientName);

        // Metadata
        services.TryAddRestClient<IAlbumService, AlbumService>(clientName);
        services.TryAddRestClient<ITrackService, TrackService>(clientName);
        services.TryAddRestClient<IAlbumCreationService, AlbumCreationService>(clientName);
        services.TryAddRestClient<IArtistCreationService, ArtistCreationService>(clientName);

        // Search
        services.TryAddRestClient<ISearchService, SearchService>(clientName);

        // Sessions
        services.TryAddRestClient<ISessionService, SessionService>(clientName);

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(services, [typeof(ISessionService).Assembly]);

        return services;
    }
}
