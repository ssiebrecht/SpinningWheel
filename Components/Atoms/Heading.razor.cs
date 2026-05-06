using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Atoms;

public partial class Heading
{
    [Parameter] public int Level { get; set; } = 2;
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
