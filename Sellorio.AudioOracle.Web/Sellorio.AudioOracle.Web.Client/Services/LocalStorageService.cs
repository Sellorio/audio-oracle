using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Sellorio.AudioOracle.Web.Client.Services;

public class LocalStorageService(IJSRuntime js) : ILocalStorageService
{
    public ValueTask SetItemAsync(string key, string value) =>
        js.InvokeVoidAsync("localStorage.setItem", key, value);

    public ValueTask<string?> GetItemAsync(string key) =>
        js.InvokeAsync<string?>("localStorage.getItem", key);

    public ValueTask RemoveItemAsync(string key) =>
        js.InvokeVoidAsync("localStorage.removeItem", key);
}
