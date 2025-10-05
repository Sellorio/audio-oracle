using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/albums/{albumId:int}/tracks")]
public class TrackController(ITrackService trackService) : ControllerBase
{
    [HttpPost("retry-all")]
    public async Task<IActionResult> RetryAllAsync(int albumId)
    {
        return await trackService.RetryAllTracksAsync(albumId).ToActionResult();
    }

    [HttpPost("{id:int}/request")]
    public async Task<IActionResult> RequestAsync(int albumId, int id)
    {
        return await trackService.RequestTrackAsync(albumId, id).ToActionResult();
    }

    [HttpPost("{id:int}/unrequest")]
    public async Task<IActionResult> UnrequestAsync(int albumId, int id, bool deleteFile)
    {
        return await trackService.UnrequestTrackAsync(albumId, id, deleteFile).ToActionResult();
    }

    [HttpPut("{id:int}/download-source")]
    public async Task<IActionResult> PutDownloadSourceAsync(int albumId, int id, DownloadSource downloadSource, bool redownloadTrack)
    {
        return await trackService.ChangeDownloadSourceAsync(albumId, id, downloadSource, redownloadTrack).ToActionResult();
    }

    [HttpDelete("{id:int}/media")]
    public async Task<IActionResult> PutDownloadSourceAsync(int albumId, int id)
    {
        return await trackService.DeleteTrackMediaAsync(albumId, id).ToActionResult();
    }
}
