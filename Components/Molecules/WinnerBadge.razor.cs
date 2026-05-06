using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Molecules;

public partial class WinnerBadge
{
    [Parameter, EditorRequired] public string Name { get; set; } = string.Empty;
}
