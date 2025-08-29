using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Web.Controllers;

internal static class ExtensionsForControllers
{
    public static IActionResult ToActionResult(
        this IResult result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest)
    {
        if (result.WasSuccess)
        {
            return new ObjectResult(result) { StatusCode = (int)successStatusCode };
        }

        return new ObjectResult(result) { StatusCode = (int)failureStatusCode };
    }

    public static async Task<IActionResult> ToActionResult(
        this Task<IResult> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest)
    {
        return ToActionResult(await result, successStatusCode, failureStatusCode);
    }

    public static async Task<IActionResult> ToActionResult(
        this Task<Result> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest)
    {
        return ToActionResult(await result, successStatusCode, failureStatusCode);
    }

    public static async Task<IActionResult> ToActionResult<TContext>(
        this Task<Result<TContext>> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest)
    {
        return ToActionResult(await result, successStatusCode, failureStatusCode);
    }

    public static async Task<IActionResult> ToActionResult<TValue>(
        this Task<ValueResult<TValue>> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest)
    {
        return ToActionResult(await result, successStatusCode, failureStatusCode);
    }

    public static async Task<IActionResult> ToActionResult<TContext, TValue>(
        this Task<ValueResult<TContext, TValue>> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        HttpStatusCode failureStatusCode = HttpStatusCode.BadRequest)
    {
        return ToActionResult(await result, successStatusCode, failureStatusCode);
    }
}
