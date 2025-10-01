using System.Threading.Tasks;
using Sellorio.AudioOracle.Library.Results;

namespace Sellorio.AudioOracle.Web.Client.Library.Services;

public interface IResultPopupService
{
    Task ShowResultAsPopupAsync(IResult result, string? successMessage);
}
