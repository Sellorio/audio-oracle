using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;

namespace Sellorio.AudioOracle.Web.Controllers;

[Authorize]
[ApiController]
[Route("artists")]
public class ArtistController(IArtistCreationService artistCreationService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync(ArtistPost[] artistPosts)
    {
        return await artistCreationService.GetOrCreateArtistsAsync(artistPosts).ToActionResult();
    }
}
