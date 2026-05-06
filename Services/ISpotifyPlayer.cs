using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Services;

/// <summary>
/// Controls a single Spotify embed iframe via the official IFrame API.
/// Lifecycle is bound to the consuming component (scoped + <see cref="IAsyncDisposable"/>).
/// </summary>
public interface ISpotifyPlayer : IAsyncDisposable
{
    bool IsReady { get; }
    bool IsPaused { get; }

    event Action? PlaybackChanged;

    ValueTask InitializeAsync(ElementReference host, string uri);
    ValueTask LoadUriAsync(string uri);
    ValueTask PlayAsync();
    ValueTask PauseAsync();
}
