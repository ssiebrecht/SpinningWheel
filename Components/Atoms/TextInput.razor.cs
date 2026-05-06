using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SpinningWheel.Components.Atoms;

public partial class TextInput
{
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public int MaxLength { get; set; } = 60;
    [Parameter] public EventCallback OnEnter { get; set; }

    private Task HandleInput(ChangeEventArgs e)
        => ValueChanged.InvokeAsync(e.Value?.ToString() ?? string.Empty);

    private async Task HandleKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && OnEnter.HasDelegate)
        {
            await OnEnter.InvokeAsync();
        }
    }
}
