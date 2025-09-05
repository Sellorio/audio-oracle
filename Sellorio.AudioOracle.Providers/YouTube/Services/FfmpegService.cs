using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class FfmpegService(HttpClient httpClient) : IFfmpegService
{
    private const string DownloadUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-lgpl.zip";
    private static readonly SemaphoreSlim semaphore = new(1);

    public async Task<Result> ConvertToMp3Async(string source, string destination, int outputBitrateKbps, bool loudnessNormalization)
    {
        await EnsureFfmpegExecutableAsync();

        // According to ChatGPT loudnorm should be -14 and TP should be -1 but for my test track these numbers felt more accurate
        var filters = loudnessNormalization ? "-af \"loudnorm=I=-12:TP=0:LRA=11\"" : "";
        var tagsFormat = "-id3v2_version 3 -write_id3v1 1";
        var bitrate = $"-ab {outputBitrateKbps}k";

        var startInfo =
            new ProcessStartInfo(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "ffmpeg.exe"),
                $"-y -i \"{source}\" {filters} {tagsFormat} {bitrate} \"{destination}\"")
            {
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

        var process = Process.Start(startInfo);

        // make sure the output buffer doesn't fill and block the process
        var errorOutputTask = process!.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            return ResultMessage.Error(await errorOutputTask);
        }

        return Result.Success();
    }

    private async Task EnsureFfmpegExecutableAsync()
    {
        if (File.Exists(Constants.FfmpegPath))
        {
            return;
        }

        await semaphore.WaitAsync();

        try
        {
            if (File.Exists(Constants.FfmpegPath)) // file was downloaded while we waited for the semaphore to unlock
            {
                return;
            }

            using var downloadStream = await httpClient.GetStreamAsync(DownloadUrl);
            using var zip = new ZipArchive(downloadStream, ZipArchiveMode.Read, true);
            var entry = zip.Entries.First(x => x.Name == "ffmpeg.exe");
            entry.ExtractToFile(Constants.FfmpegPath);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
