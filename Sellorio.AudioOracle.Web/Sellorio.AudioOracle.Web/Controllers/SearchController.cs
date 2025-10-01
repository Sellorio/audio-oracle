using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.ServiceInterfaces.Search;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/search")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SearchAsync(string searchText)
    {
        return await searchService.SearchAsync(searchText).ToActionResult();
    }

    [HttpPost("download")]
    public async Task<IActionResult> SearchDownloadAsync(int trackId)
    {
        return await searchService.SearchForDownloadAsync(trackId).ToActionResult();
    }

    [HttpPost("download-url")]
    public async Task<IActionResult> SearchDownloadUrlAsync(string url)
    {
        return await searchService.SearchForDownloadByUrlAsync(url).ToActionResult();
    }
}
