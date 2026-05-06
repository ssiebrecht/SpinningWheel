using Microsoft.AspNetCore.Components;
using SpinningWheel.Services;

namespace SpinningWheel.Components.Organisms;

public partial class NameList : IDisposable
{
    [Inject] private INameStore Store { get; set; } = default!;

    protected override void OnInitialized()
    {
        Store.OnChanged += HandleStoreChanged;
    }

    private void HandleStoreChanged() => InvokeAsync(StateHasChanged);

    private Task HandleRemoveAsync(Guid id) => Store.RemoveAsync(id);

    private Task HandleClearAsync() => Store.ClearAsync();

    public void Dispose()
    {
        Store.OnChanged -= HandleStoreChanged;
    }
}
