using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Atoms;

public partial class WheelPlaceholder
{
    [Parameter, EditorRequired] public double Radius { get; set; }
    [Parameter] public string Label { get; set; } = "Namen hinzufügen";
}
