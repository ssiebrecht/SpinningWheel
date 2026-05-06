using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Atoms;

public partial class PrimaryButton
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool IsDanger { get; set; }
    [Parameter] public string Type { get; set; } = "button";
}
