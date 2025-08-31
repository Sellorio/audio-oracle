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
    public async Task<IActionResult> PostAsync(string searchText)
    {
        return await searchService.SearchAsync(searchText).ToActionResult();
    }
}
