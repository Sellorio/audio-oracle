using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Providers.SoundCloud.Models;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Services;

internal interface ISoundCloudApiService
{
    Task<Result> DownloadTrackAsync(TrackDto track, string outputFilename);
    Task<TrackDto?> GetTrackAsync(long id);
    Task<TrackDto?> GetTrackAsync(string url);
    Task<SearchResultDto<TrackDto>> SearchTracksAsync(string q);
}