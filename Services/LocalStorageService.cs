using System.Text.Json;
using Microsoft.JSInterop;

namespace WordKeeper.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        string json = JsonSerializer.Serialize(value);

        await _jsRuntime.InvokeVoidAsync(
            "localStorageHelper.setItem",
            key,
            json
        );
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        string? json = await _jsRuntime.InvokeAsync<string?>(
            "localStorageHelper.getItem",
            key
        );

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveItemAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync(
            "localStorageHelper.removeItem",
            key
        );
    }
}