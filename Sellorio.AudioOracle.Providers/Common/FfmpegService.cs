using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.YouTube;

namespace Sellorio.AudioOracle.Providers.Common;

internal class FfmpegService : IFfmpegService
{
    private static readonly SemaphoreSlim semaphore = new(1);

    public async Task<Result> ConvertToMp3Async(string source, string destination, int outputBitrateKbps, bool loudnessNormalization)
    {
        await EnsureFfmpegExecutableAsync();

        var destinationDirectory = Path.GetDirectoryName(destination)!;

        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        // According to ChatGPT loudnorm should be -14 and TP should be -1 but for my test track these numbers felt more accurate
        var filters = loudnessNormalization ? "-af \"loudnorm=I=-12:TP=0:LRA=11\"" : "";
        var tagsFormat = "-id3v2_version 3 -write_id3v1 1";
        var bitrate = $"-ab {outputBitrateKbps}k";

        var startInfo = new ProcessStartInfo(Constants.FfmpegPath, $"-y -i \"{source}\" {filters} {tagsFormat} {bitrate} \"{destination}\"")
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

    public async Task EnsureFfmpegExecutableAsync()
    {
        if (File.Exists(Constants.FfmpegPath))
        {
            return;
        }

        await semaphore.WaitAsync();

        try
        {
            if (File.Exists(Constants.FfmpegPath)) // file was "downloaded" while we waited for the semaphore to unlock
            {
                return;
            }

            // Wanted to download FFMPEG from https://github.com/BtbN/FFmpeg-Builds at runtime but unzipping a .tar.xz file is seamingly imposssible
            using (var manifestStream = typeof(FfmpegService).Assembly.GetManifestResourceStream(typeof(FfmpegService).Namespace + ".ffmpeg.zip")!)
            using (var zipArchive = new ZipArchive(manifestStream))
            {
                zipArchive.Entries[0].ExtractToFile(Constants.FfmpegPath);
            }

            await Process.Start("chmod", $"+x {Constants.FfmpegPath}")!.WaitForExitAsync();
        }
        finally
        {
            semaphore.Release();
        }
    }
}
