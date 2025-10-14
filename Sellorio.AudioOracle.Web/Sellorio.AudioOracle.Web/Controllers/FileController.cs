using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Library.Results.Messages;
using Sellorio.AudioOracle.ServiceInterfaces.Content;
using Sellorio.AudioOracle.Services.Content;

namespace Sellorio.AudioOracle.Web.Controllers;

[ApiController]
public class FileController(IFileService fileService, IDataFileService dataFileService, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(FileController));

    [HttpGet("f/{urlId}")] // f = File. Any file that is stored in the database
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

        Response.Headers.ContentDisposition = $"inline; filename=\"{RemoveNonAscii(result.Value.FileName)}\"; filename*=UTF-8''{Uri.EscapeDataString(result.Value.FileName)}";

        return File(result.Value.Stream, "audio/mpeg");
    }

    [HttpPost("api/df")] // df = data file. Endpoint used to provide cookies-youtube.txt for use in the YouTube provider
    public async Task<IActionResult> PostDataFileAsync(string fileName, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ((Result)ResultMessage.Error("Missing file.")).ToActionResult();
        }

        using var stream = file.OpenReadStream();
        return await dataFileService.PostDataFileAsync(fileName, file.Length, stream).ToActionResult();
    }

    private string RemoveNonAscii(string input)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in input)
            if (c <= 127) sb.Append(c);
        return sb.ToString();
    }
}
