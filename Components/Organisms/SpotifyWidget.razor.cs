using Microsoft.AspNetCore.Components;
using SpinningWheel.Models;
using SpinningWheel.Services;

namespace SpinningWheel.Components.Organisms;

public partial class SpotifyWidget : IAsyncDisposable
{
    private const string StorageKey = "spinning-wheel.spotify-uri.v1";

    [Inject] private ILocalStorageService Storage { get; set; } = default!;
    [Inject] private ISpinBroadcaster SpinBroadcaster { get; set; } = default!;
    [Inject] private ISpotifyPlayer Player { get; set; } = default!;

    private bool _isOpen;
    private bool _isConfiguring;
    private string _currentUri = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var saved = await Storage.GetAsync<string>(StorageKey);
        if (!string.IsNullOrWhiteSpace(saved))
        {
            _currentUri = saved;
        }

        SpinBroadcaster.SpinStarted += HandleSpinStarted;
        SpinBroadcaster.WinnerRevealed += HandleWinnerRevealed;
    }

    private async Task HandleToggleAsync()
    {
        _isOpen = !_isOpen;
        if (!_isOpen)
        {
            _isConfiguring = false;
        }
        await Task.CompletedTask;
    }

    private async Task HandleUriSubmittedAsync(string uri)
    {
        _currentUri = uri;
        _isConfiguring = false;
        await Storage.SetAsync(StorageKey, uri);
    }

    private Task HandleChangeUriAsync()
    {
        _isConfiguring = true;
        return Task.CompletedTask;
    }

    private Task HandleCancelConfigureAsync()
    {
        _isConfiguring = false;
        return Task.CompletedTask;
    }

    private void HandleSpinStarted()
    {
        if (string.IsNullOrEmpty(_currentUri) || !Player.IsReady)
        {
            return;
        }

        _ = InvokeAsync(PlaySafelyAsync);
    }

    private async Task PlaySafelyAsync()
    {
        try
        {
            await Player.PlayAsync();
        }
        catch
        {
            // Autoplay can be blocked by the browser before user interaction.
            // We silently ignore; the user can still start manually.
        }
    }

    private void HandleWinnerRevealed(WheelEntry winner)
    {
        _ = winner;
        if (!Player.IsReady)
        {
            return;
        }

        _ = InvokeAsync(PauseSafelyAsync);
    }

    private async Task PauseSafelyAsync()
    {
        try
        {
            await Player.PauseAsync();
        }
        catch
        {
            // Ignore – player may have been torn down.
        }
    }

    public async ValueTask DisposeAsync()
    {
        SpinBroadcaster.SpinStarted -= HandleSpinStarted;
        SpinBroadcaster.WinnerRevealed -= HandleWinnerRevealed;
        await Player.DisposeAsync();
    }
}
