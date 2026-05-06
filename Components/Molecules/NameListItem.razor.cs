using Microsoft.AspNetCore.Components;
using SpinningWheel.Models;

namespace SpinningWheel.Components.Molecules;

public partial class NameListItem
{
    [Parameter, EditorRequired] public WheelEntry Entry { get; set; } = default!;
    [Parameter] public EventCallback<Guid> OnRemove { get; set; }

    private Task HandleRemoveAsync() => OnRemove.InvokeAsync(Entry.Id);
}
