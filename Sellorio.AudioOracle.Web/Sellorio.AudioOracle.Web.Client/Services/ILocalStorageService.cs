using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Web.Client.Services;

public interface ILocalStorageService
{
    ValueTask<string?> GetItemAsync(string key);
    ValueTask RemoveItemAsync(string key);
    ValueTask SetItemAsync(string key, string value);
}