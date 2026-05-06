using Microsoft.AspNetCore.Components;
using SpinningWheel.Models;
using SpinningWheel.Services;

namespace SpinningWheel.Components.Pages;

public partial class Home : IDisposable
{
    [Inject] private INameStore Store { get; set; } = default!;
    [Inject] private IQuoteService QuoteService { get; set; } = default!;

    private WheelEntry? _winner;
    private string? _quote;
    private bool _initialLoadDone;

    protected override void OnInitialized()
    {
        Store.OnChanged += HandleStoreChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_initialLoadDone)
        {
            _initialLoadDone = true;
            await Store.LoadAsync();
        }
    }

    private void HandleStoreChanged() => InvokeAsync(StateHasChanged);

    private Task HandleAddNameAsync(string name) => Store.AddAsync(name);

    private Task HandleWinnerAsync(WheelEntry winner)
    {
        _winner = winner;
        _quote = QuoteService.GetRandom();
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleCloseWinnerAsync()
    {
        _winner = null;
        _quote = null;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Store.OnChanged -= HandleStoreChanged;
    }
}
