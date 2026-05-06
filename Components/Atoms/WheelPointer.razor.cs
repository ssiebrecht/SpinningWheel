using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Atoms;

public partial class WheelPointer
{
    [Parameter, EditorRequired] public double Radius { get; set; }
}
