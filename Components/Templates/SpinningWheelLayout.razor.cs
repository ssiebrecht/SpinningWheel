using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Templates;

public partial class SpinningWheelLayout
{
    [Parameter] public RenderFragment? Header { get; set; }
    [Parameter] public RenderFragment? Input { get; set; }
    [Parameter] public RenderFragment? Wheel { get; set; }
    [Parameter] public RenderFragment? Names { get; set; }
}
