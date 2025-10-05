using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.Common;
using Sellorio.AudioOracle.Providers.SoundCloud.Models;

namespace Sellorio.AudioOracle.Providers.SoundCloud.Services;

internal class SoundCloudApiService(HttpClient httpClient, IFfmpegService ffmpegService, ISoundCloudClientIdService soundCloudClientIdService) : ISoundCloudApiService
{
    private static readonly JsonSerializerOptions _jsonOptions;

    private static readonly MemoryCache _trackCache = new(new MemoryCacheOptions());
    private static readonly TimeSpan _trackCacheDuration = TimeSpan.FromDays(7);

    static SoundCloudApiService()
    {
        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async Task<TrackDto?> GetTrackAsync(long id)
    {
        return await ProviderHelper.GetWithCacheAsync(_trackCache, _trackCacheDuration, id, async id =>
        {
            try
            {
                var clientId = await soundCloudClientIdService.GetClientIdAsync();
                return (await httpClient.GetFromJsonAsync<TrackDto>($"tracks/{id}?client_id={clientId}", _jsonOptions))!;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        });
    }

    public async Task<TrackDto?> GetTrackAsync(string url)
    {
        try
        {
            var clientId = await soundCloudClientIdService.GetClientIdAsync();
            var track = await httpClient.GetFromJsonAsync<TrackDto>($"resolve?url={WebUtility.UrlEncode(url)}&client_id={clientId}", _jsonOptions)!;

            _ = _trackCache.GetOrCreate(track!.Id, x => new Lazy<Task<TrackDto>>(Task.FromResult(track)), new()
            {
                AbsoluteExpiration = DateTimeOffset.Now + _trackCacheDuration
            });

            return track;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<SearchResultDto<TrackDto>> SearchTracksAsync(string q)
    {
        var clientId = await soundCloudClientIdService.GetClientIdAsync();
        var result = await httpClient.GetFromJsonAsync<SearchResultDto<TrackDto>>($"search/tracks?client_id={clientId}&q={WebUtility.UrlEncode(q)}", _jsonOptions);
        var now = DateTimeOffset.Now;

        foreach (var track in result!.Collection)
        {
            _ = _trackCache.GetOrCreate(track.Id, x => new Lazy<Task<TrackDto>>(Task.FromResult(track)), new()
            {
                AbsoluteExpiration = now + _trackCacheDuration
            });
        }

        return result;
    }

    public async Task<Result> DownloadTrackAsync(TrackDto track, string outputFilename)
    {
        var bestTranscode = track.Media.Transcodings.First();

        var outputDirectory = Path.GetDirectoryName(outputFilename)!;

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        if (bestTranscode.Format.Protocol is "hls" or "progressive")
        {
            var clientId = await soundCloudClientIdService.GetClientIdAsync();
            var transcodeInfo = await httpClient.GetFromJsonAsync<TranscodeInfoDto>(bestTranscode.Url + "?client_id=" + clientId, _jsonOptions);
            var downloadUrl = transcodeInfo!.Url;

            return await ffmpegService.DownloadMediaStreamAsync(downloadUrl, outputFilename, outputBitrateKbps: 256, loudnessNormalization: true, reEncodeToMp3: bestTranscode.Format.Protocol == "hls");
        }
        //if (bestTranscode.Format.Protocol == "hls")
        //{
        //    return await DownloadTrackUsingHlsAsync(bestTranscode.Url, outputFilename);
        //}
        //else if (bestTranscode.Format.Protocol == "progressive")
        //{
        //    await DownloadTrackUsingProgressiveAsync(bestTranscode.Url, outputFilename);
        //}
        else
        {
            return ResultMessage.Critical("Unexpected format protocol when downloading from SoundCloud.");
        }

        //return Result.Success();
    }

    //private async Task<Result> DownloadTrackUsingHlsAsync(string transcodeUrl, string outputFilename)
    //{
    //    var clientId = await soundCloudClientIdService.GetClientIdAsync();
    //    var transcodeInfo = await httpClient.GetFromJsonAsync<TranscodeInfoDto>(transcodeUrl + "?client_id=" + clientId, _jsonOptions);
    //    var downloadUrl = transcodeInfo!.Url;

    //    return await ffmpegService.DownloadMediaStreamAsync(downloadUrl, outputFilename, outputBitrateKbps: 256, loudnessNormalization: true);
    //}

    //private async Task DownloadTrackUsingProgressiveAsync(string transcodeUrl, string outputFilename)
    //{
    //    var clientId = await soundCloudClientIdService.GetClientIdAsync();
    //    var transcodeInfo = await httpClient.GetFromJsonAsync<TranscodeInfoDto>(transcodeUrl + "?client_id=" + clientId, _jsonOptions);
    //    var downloadUrl = transcodeInfo!.Url;

    //    using var downloadStream = await httpClient.GetStreamAsync(downloadUrl);
    //    using var fileStream = new FileStream(outputFilename, FileMode.Create, FileAccess.Write);

    //    await downloadStream.CopyToAsync(fileStream);

    //    #error use ffmpeg conversion
    //}
}
