using Microsoft.AspNetCore.Components;
using SpinningWheel.Models;

namespace SpinningWheel.Components.Organisms;

public partial class WinnerOverlay
{
    [Parameter] public WheelEntry? Entry { get; set; }
    [Parameter] public string? Quote { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }

    private Task HandleCloseAsync() => OnClose.InvokeAsync();

    private Task HandleBackdropClick() => OnClose.InvokeAsync();
}
