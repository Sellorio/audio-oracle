using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.ServiceInterfaces.Providers;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/providers")]
public class ProviderController(IProviderCatalogService providerCatalogService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        return await providerCatalogService.GetProvidersInfoAsync().ToActionResult();
    }
}
