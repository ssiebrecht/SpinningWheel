using System.Text.Json;
using Microsoft.JSInterop;

namespace SpinningWheel.Services;

/// <inheritdoc />
public sealed class LocalStorageService(IJSRuntime js) : ILocalStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var raw = await js.InvokeAsync<string?>("localStorage.getItem", key);
        if (string.IsNullOrEmpty(raw))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(raw, JsonOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public async ValueTask SetAsync<T>(string key, T value)
    {
        var raw = JsonSerializer.Serialize(value, JsonOptions);
        await js.InvokeVoidAsync("localStorage.setItem", key, raw);
    }

    public ValueTask RemoveAsync(string key)
        => js.InvokeVoidAsync("localStorage.removeItem", key);
}
