using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sellorio.AudioOracle.Services.Sessions;

namespace Sellorio.AudioOracle.Web.Framework;

internal class AuthorizeFilter(ISessionService sessonService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Headers.Authorization.Count != 1)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var authorizationValue = context.HttpContext.Request.Headers.Authorization[0];
        var authorizationValueParts = authorizationValue.Split(' ');

        var scheme = authorizationValueParts[0];
        var sessionToken = authorizationValueParts[1];

        if (scheme != "aoid")
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var result = await sessonService.UseSessionAsync(sessionToken);

        if (!result.WasSuccess)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next.Invoke();
    }
}
