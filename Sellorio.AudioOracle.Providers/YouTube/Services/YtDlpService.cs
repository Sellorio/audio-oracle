using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;

namespace Sellorio.AudioOracle.Providers.YouTube.Services;

internal class YtDlpService(HttpClient httpClient) : IYtDlpService
{
    private const string ExePath = "/data/yt-dlp.exe";
    private static readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
    private static readonly SemaphoreSlim _semaphore = new(1);
    private static DateTime _lastChecked;

    public async Task<Result> InvokeYtDlpAsync(string arguments, string outputDirectory = "")
    {
        await _semaphore.WaitAsync();

        try
        {
            await EnsureLatestYtDlpExecutableAsync();

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
            if (_lastChecked + _checkInterval > DateTime.Now)
            {
                return;
            }

            var version = FileVersionInfo.GetVersionInfo(ExePath).FileVersion;
            var client = new GitHubClient(new ProductHeaderValue(ProviderConstants.UserAgent));
            var releases = await client.Repository.Release.GetAll("yt-dlp", "yt-dlp", new() { StartPage = 1, PageSize = 1 });
            var release = releases[0];

            // we have the latest vesion
            if (release.TagName == version)
            {
                _lastChecked = DateTime.Now;
                return;
            }
        }

        using var fileStream = new FileStream(ExePath, System.IO.FileMode.Create, FileAccess.Write);
        var downloadStream = await httpClient.GetStreamAsync("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe");
        await downloadStream.CopyToAsync(fileStream);
    }
}
