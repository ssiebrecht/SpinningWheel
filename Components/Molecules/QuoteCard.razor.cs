using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Molecules;

public partial class QuoteCard
{
    [Parameter] public string? Quote { get; set; }
}
