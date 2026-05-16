using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sellorio.AudioOracle.Client.Content;
using Sellorio.AudioOracle.Client.Events;
using Sellorio.AudioOracle.Client.Internal;
using Sellorio.AudioOracle.Client.Metadata;
using Sellorio.AudioOracle.Client.Providers;
using Sellorio.AudioOracle.Client.Search;
using Sellorio.AudioOracle.Client.Sessions;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.ServiceInterfaces.Events;
using Sellorio.AudioOracle.ServiceInterfaces.Content;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Providers;
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
        services.AddSingleton<ISignalREventClient>(svc => new SignalREventClient(baseAddress, svc.GetRequiredService<IAudioOracleSessionTokenProvider>(), svc.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SignalREventClient>>()));
        AddEventServices(services);

        // Content
        services.TryAddRestClient<IDataFileService, DataFileService>(clientName);

        // Metadata
        services.TryAddRestClient<IAlbumService, AlbumService>(clientName);
        services.TryAddRestClient<ITrackService, TrackService>(clientName);
        services.TryAddRestClient<IAlbumCreationService, AlbumCreationService>(clientName);
        services.TryAddRestClient<IArtistCreationService, ArtistCreationService>(clientName);

        // Providers
        services.TryAddRestClient<IProviderCatalogService, ProviderCatalogService>(clientName);

        // Search
        services.TryAddRestClient<ISearchService, SearchService>(clientName);

        // Sessions
        services.TryAddRestClient<ISessionService, SessionService>(clientName);

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(services, [typeof(ISessionService).Assembly]);

        return services;
    }

    private static void AddEventServices(IServiceCollection services)
    {
        var clientAssembly = typeof(ServiceCollectionExtensions).Assembly;

        foreach (var eventInterfaceType in EventContractRegistry.GetEventInterfaceTypes())
        {
            var implementationType =
                clientAssembly.GetTypes().SingleOrDefault(x =>
                    x is { IsClass: true, IsAbstract: false } &&
                    eventInterfaceType.IsAssignableFrom(x));

            if (implementationType == null)
            {
                throw new InvalidOperationException($"No client implementation was found for '{eventInterfaceType.FullName}'.");
            }

            services.AddSingleton(eventInterfaceType, implementationType);
        }
    }
}
