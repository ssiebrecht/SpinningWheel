using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Atoms;

public partial class IconButton
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public string AriaLabel { get; set; } = "action";
}
