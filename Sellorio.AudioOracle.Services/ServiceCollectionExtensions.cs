using System.Net.Http;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sellorio.AudioOracle.Library.DependencyInjection;
using Sellorio.AudioOracle.Models.TaskQueue;
using Sellorio.AudioOracle.Providers.MusicBrainz;
using Sellorio.AudioOracle.Providers.SoundCloud;
using Sellorio.AudioOracle.Providers.YouTube;
using Sellorio.AudioOracle.ServiceInterfaces.Content;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Providers;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;
using Sellorio.AudioOracle.Services.Content;
using Sellorio.AudioOracle.Services.Import;
using Sellorio.AudioOracle.Services.Metadata;
using Sellorio.AudioOracle.Services.Providers;
using Sellorio.AudioOracle.Services.Search;
using Sellorio.AudioOracle.Services.Sessions;
using Sellorio.AudioOracle.Services.TaskQueue;
using Sellorio.AudioOracle.Services.TaskQueue.Handlers;
using Sellorio.AudioOracle.Services.TaskQueue.Queuers;

namespace Sellorio.AudioOracle.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAudioOracleServerSideServices(this IServiceCollection services)
    {
        services
            .AddHttpClient("NoSSL")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });

        services
            .AddYouTubeProvider()
            .AddSoundCloudProvider()
            .AddMusicBrainzProvider()

            .AddScoped<SessionState>()
            .AddScoped<IProviderInvocationService, ProviderInvocationService>()

            // Content
            .AddScoped<IContentMapper, ContentMapper>()
            .AddScoped<IDataFileService, DataFileService>()
            .AddScoped<IFileService, FileService>()
            .AddScoped<IFileTagsService, FileTagsService>()

            // Import
            .AddScoped<IImportService, ImportService>()

            // Metadata
            .AddScoped<IMetadataMapper, MetadataMapper>()
            .AddScoped<IAlbumService, AlbumService>()
            .AddScoped<ITrackService, TrackService>()
            .AddScoped<IArtistCreationService, ArtistCreationService>()
            .AddScoped<IAlbumCreationService, AlbumCreationService>()

            // Providers
            .AddScoped<IProviderCatalogService, ProviderCatalogService>()

            // Search
            .AddScoped<ISearchService, SearchService>()

            // Sessions
            .AddScoped<ISessionService, SessionService>()

            // Task Queue
            .AddSingleton(Channel.CreateUnbounded<QueuedTask>())
            .AddSingleton<ITaskQueueService, TaskQueueService>()

            .AddScoped<ITaskQueueMapper, TaskQueueMapper>()
            .AddScoped<ITaskQueuingService, TaskQueuingService>()
                
                // Handlers
                .AddScoped<ITaskHandler, TrackMetadata>()
                .AddScoped<ITaskHandler, DownloadTrack>()

                // Queuers
                .AddScoped<ITrackMetadataTaskQueuingService, TrackMetadataTaskQueuingService>()
                .AddScoped<IDownloadTrackTaskQueuingService, DownloadTrackTaskQueuingService>()
            ;

        ServiceRegistrationHelper.EnsureAllServicesAreRegistered(
            services,
            [typeof(ISessionService).Assembly, typeof(ServiceCollectionExtensions).Assembly]);

        return services;
    }
}
