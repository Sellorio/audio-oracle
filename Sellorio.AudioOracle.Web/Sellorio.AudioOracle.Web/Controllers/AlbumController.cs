using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Models;
using Sellorio.AudioOracle.Models.Content;
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

    [HttpGet("page")]
    public async Task<IActionResult> PageAsync(int pageNumber, int pageSize, bool onlyAlbumsRequiringAttention = false, AlbumFields include = AlbumFields.None)
    {
        return await albumService.GetAlbumPageAsync(pageNumber, pageSize, onlyAlbumsRequiringAttention, include).ToActionResult();
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

    [HttpPut("{id:int}/art")]
    public async Task<IActionResult> PutArtAsync(int id, FileType imageType, [FromForm] IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return await albumService.UpdateAlbumArtAsync(id, imageType, stream).ToActionResult();
    }
}
