using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Molecules;

public partial class SpotifyUriForm
{
    // Accepts https://open.spotify.com/{type}/{id} and spotify:{type}:{id}.
    private static readonly Regex SpotifyUrlPattern = new(
        @"^(https?://open\.spotify\.com/(intl-[a-z-]+/)?(track|playlist|album|episode|show)/[A-Za-z0-9]+(\?.*)?|spotify:(track|playlist|album|episode|show):[A-Za-z0-9]+)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private string _currentUri = string.Empty;
    private string? _errorMessage;

    [Parameter] public string? InitialUri { get; set; }
    [Parameter] public EventCallback<string> OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private bool CanSubmit => !string.IsNullOrWhiteSpace(_currentUri);

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrEmpty(InitialUri) && string.IsNullOrEmpty(_currentUri))
        {
            _currentUri = InitialUri;
        }
    }

    private Task OnValueChanged(string value)
    {
        _currentUri = value;
        _errorMessage = null;
        return Task.CompletedTask;
    }

    private async Task HandleSubmitAsync()
    {
        if (!CanSubmit)
        {
            return;
        }

        var value = _currentUri.Trim();
        if (!SpotifyUrlPattern.IsMatch(value))
        {
            _errorMessage = "Bitte einen gültigen Spotify-Link einfügen (Track, Playlist, Album, …).";
            return;
        }

        _errorMessage = null;
        await OnSubmit.InvokeAsync(value);
    }

    private Task HandleCancelAsync() => OnCancel.InvokeAsync();
}
