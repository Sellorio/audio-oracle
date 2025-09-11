using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Models.Content;
using Sellorio.AudioOracle.Services.Content;

namespace Sellorio.AudioOracle.Web.Controllers;

[ApiController]
public class FileController(IFileService fileService, HttpClient httpClient) : ControllerBase
{
    [Authorize]
    [HttpGet("f/{urlId}")]
    public async Task<IActionResult> GetFileContentAsync(string urlId)
    {
        var fileResult = await fileService.GetByUrlIdAsync(urlId);

        if (fileResult.WasSuccess)
        {
            var mimeType = fileResult.Value.Type switch
            {
                FileType.ImageJpeg => MediaTypeNames.Image.Jpeg,
                FileType.ImagePng => MediaTypeNames.Image.Png,
                FileType.Unspecified => MediaTypeNames.Application.Octet,
                _ => throw new NotSupportedException("Unexpected file type.")
            };

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

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return
            File(
                await response.Content.ReadAsStreamAsync(),
                response.Content.Headers.ContentType.MediaType);
    }
}
