using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
}
