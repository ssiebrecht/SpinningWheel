namespace SpinningWheel.Services;

/// <summary>
/// Thin wrapper around the browser's <c>localStorage</c>.
/// </summary>
public interface ILocalStorageService
{
    ValueTask<T?> GetAsync<T>(string key);
    ValueTask SetAsync<T>(string key, T value);
    ValueTask RemoveAsync(string key);
}
