using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.Providers.Common;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class YtDlpService(HttpClient httpClient, IFfmpegService ffmpegService) : IYtDlpService
{
    private const string ExePath = "/data/yt-dlp";
    private static readonly SemaphoreSlim _semaphore = new(1);

    public async Task<Result> InvokeYtDlpAsync(string arguments, string outputDirectory = "")
    {
        await _semaphore.WaitAsync();

        try
        {
            await EnsureLatestYtDlpExecutableAsync();

            if (arguments.Contains("ffmpeg"))
            {
                await ffmpegService.EnsureFfmpegExecutableAsync();
            }

            var startInfo = new ProcessStartInfo(ExePath, arguments)
            {
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = outputDirectory
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();

            await process!.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var errorOutput = await process.StandardError.ReadToEndAsync();

                if (Regex.IsMatch(errorOutput, @$"^ERROR: \[youtube\] {Constants.YouTubeIdRegex}: Video unavailable. This video is not available"))
                {
                    return ResultMessage.Error("The YouTube Video is unavailable and cannot be downloaded.");
                }

                return ResultMessage.Error(errorOutput);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return Result.Success();
    }

    private async Task EnsureLatestYtDlpExecutableAsync()
    {
        if (File.Exists(ExePath))
        {
            return;
        }

        using var fileStream = new FileStream(ExePath, FileMode.Create, FileAccess.Write);
        var downloadStream = await httpClient.GetStreamAsync("https://github.com/yt-dlp/yt-dlp/releases/download/2025.09.23/yt-dlp_linux");
        await downloadStream.CopyToAsync(fileStream);
        await Process.Start("chmod", $"+x {ExePath}")!.WaitForExitAsync();
    }
}
