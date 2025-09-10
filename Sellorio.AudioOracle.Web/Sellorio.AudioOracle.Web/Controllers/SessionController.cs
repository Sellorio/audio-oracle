using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Library.Results;
using Sellorio.AudioOracle.Models.Sessions;
using Sellorio.AudioOracle.ServiceInterfaces.Sessions;

namespace Sellorio.AudioOracle.Web.Controllers;

[ApiController]
[Route("api/sessions")]
public class SessionController(ISessionService sessionService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginPost model)
    {
        return await sessionService.LoginAsync(model.Password).ToActionResult();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        return await sessionService.LogoutAsync().ToActionResult();
    }

    [Authorize]
    [HttpGet("ping")]
    public Task<IActionResult> PingAsync()
    {
        return Task.FromResult(Result.Success().ToActionResult());
    }
}
