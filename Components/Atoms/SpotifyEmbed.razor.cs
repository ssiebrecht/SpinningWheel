using Microsoft.AspNetCore.Components;
using SpinningWheel.Services;

namespace SpinningWheel.Components.Atoms;

public partial class SpotifyEmbed : IAsyncDisposable
{
    [Inject] private ISpotifyPlayer Player { get; set; } = default!;

    [Parameter, EditorRequired] public string Uri { get; set; } = string.Empty;

    private ElementReference _host;
    private string? _initializedUri;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (string.IsNullOrWhiteSpace(Uri))
        {
            return;
        }

        if (firstRender)
        {
            _initializedUri = Uri;
            await Player.InitializeAsync(_host, Uri);
            return;
        }

        if (_initializedUri != Uri)
        {
            _initializedUri = Uri;
            if (Player.IsReady)
            {
                await Player.LoadUriAsync(Uri);
            }
            else
            {
                await Player.InitializeAsync(_host, Uri);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        // Player lifetime is owned by the scoped DI container; disposing it
        // here would break other consumers. We only drop our reference.
        await ValueTask.CompletedTask;
    }
}
