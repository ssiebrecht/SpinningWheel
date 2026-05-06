using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SpinningWheel.Services;

/// <inheritdoc />
public sealed class SpotifyPlayer(IJSRuntime js) : ISpotifyPlayer
{
    private const string ModulePath = "./js/spotifyEmbed.js";

    private IJSObjectReference? _module;
    private IJSObjectReference? _handle;
    private DotNetObjectReference<SpotifyPlayerCallbacks>? _callbacksRef;

    public bool IsReady => _handle is not null;
    public bool IsPaused { get; private set; } = true;

    public event Action? PlaybackChanged;

    public async ValueTask InitializeAsync(ElementReference host, string uri)
    {
        // Tear down any previous controller so a URI change re-initialises cleanly.
        await DisposeHandleAsync();

        _module ??= await js.InvokeAsync<IJSObjectReference>("import", ModulePath);

        var callbacks = new SpotifyPlayerCallbacks(this);
        _callbacksRef = DotNetObjectReference.Create(callbacks);

        _handle = await _module.InvokeAsync<IJSObjectReference>(
            "createController", host, uri, _callbacksRef);
    }

    public async ValueTask LoadUriAsync(string uri)
    {
        if (_module is null || _handle is null)
        {
            return;
        }

        await _module.InvokeVoidAsync("loadUri", _handle, uri);
    }

    public async ValueTask PlayAsync()
    {
        if (_module is null || _handle is null)
        {
            return;
        }

        await _module.InvokeVoidAsync("play", _handle);
    }

    public async ValueTask PauseAsync()
    {
        if (_module is null || _handle is null)
        {
            return;
        }

        await _module.InvokeVoidAsync("pause", _handle);
    }

    internal void HandlePlaybackUpdate(bool isPaused)
    {
        if (IsPaused == isPaused)
        {
            return;
        }

        IsPaused = isPaused;
        PlaybackChanged?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeHandleAsync();

        if (_module is not null)
        {
            try { await _module.DisposeAsync(); } catch (Exception) { /* teardown – safe to ignore */ }
            _module = null;
        }
    }

    private async ValueTask DisposeHandleAsync()
    {
        if (_handle is not null && _module is not null)
        {
            // The controller's iframe may already have been removed from the DOM
            // (e.g. when the embed component was disposed before re-initialisation).
            // Catch all exceptions so cleanup never crashes the app.
            try { await _module.InvokeVoidAsync("destroy", _handle); } catch (Exception) { }
            try { await _handle.DisposeAsync(); } catch (Exception) { }
            _handle = null;
        }

        _callbacksRef?.Dispose();
        _callbacksRef = null;
    }

    /// <summary>
    /// Thin callback target invoked from JS. Kept internal so the public
    /// surface of <see cref="SpotifyPlayer"/> stays free of [JSInvokable].
    /// </summary>
    internal sealed class SpotifyPlayerCallbacks(SpotifyPlayer owner)
    {
        [JSInvokable]
        public void OnPlaybackUpdate(bool isPaused, double position)
        {
            _ = position;
            owner.HandlePlaybackUpdate(isPaused);
        }
    }
}
