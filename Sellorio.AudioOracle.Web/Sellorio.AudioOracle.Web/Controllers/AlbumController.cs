using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Models.Metadata;
using Sellorio.AudioOracle.ServiceInterfaces.Metadata;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/albums")]
public class AlbumController(IAlbumService albumService, IAlbumCreationService albumCreationService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostFromSearchAsync(AlbumPost albumPost)
    {
        return await albumCreationService.CreateAlbumAsync(albumPost).ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync(AlbumFields include = AlbumFields.None)
    {
        return await albumService.GetAlbumsAsync(include).ToActionResult();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync(int id, AlbumFields include = AlbumFields.None)
    {
        return await albumService.GetAlbumAsync(id, include).ToActionResult();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, bool deleteFiles = true)
    {
        return await albumService.DeleteAlbumAsync(id, deleteFiles).ToActionResult();
    }
}
