using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Services.Content;

namespace Sellorio.AudioOracle.Web.Controllers;

[ApiController]
public class FileController(IFileService fileService, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(FileController));

    [HttpGet("f/{urlId}")]
    public async Task<IActionResult> GetFileContentAsync(string urlId)
    {
        var fileResult = await fileService.GetByUrlIdAsync(urlId);

        if (fileResult.WasSuccess)
        {
            var mimeType = fileResult.Value.MimeType;
            return File(fileResult.Value.Content.Data, mimeType);
        }
        else if (fileResult.Messages.Any(x => x.Severity == Library.Results.Messages.ResultMessageSeverity.NotFound))
        {
            return NotFound();
        }
        else
        {
            return fileResult.ToActionResult();
        }
    }

    [HttpGet("pf")] // pf = proxyfile. Google rate limits image downloads if they have a referrer set
    public async Task<IActionResult> GetFileFromUrlAsync(string url)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new(url),
            Method = HttpMethod.Get
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return
            File(
                await response.Content.ReadAsStreamAsync(),
                response.Content.Headers.ContentType.MediaType);
    }

    [HttpGet("mf/{trackId:int}")] // mf = Media File - the media file for a track by track id
    public async Task<IActionResult> GetMediaFileAsync(int trackId)
    {
        var result = await fileService.GetTrackMediaStreamAsync(trackId);

        if (!result.WasSuccess)
        {
            return NotFound();
        }

        return File(result.Value.Stream, "audio/mpeg", result.Value.FileName);
    }
}
