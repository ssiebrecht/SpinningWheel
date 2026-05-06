using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Molecules;

public partial class NameInputForm
{
    private string _currentName = string.Empty;

    [Parameter] public EventCallback<string> OnSubmit { get; set; }

    private bool CanSubmit => !string.IsNullOrWhiteSpace(_currentName);

    private Task OnValueChanged(string value)
    {
        _currentName = value;
        return Task.CompletedTask;
    }

    private async Task HandleSubmitAsync()
    {
        if (!CanSubmit)
        {
            return;
        }

        var value = _currentName.Trim();
        _currentName = string.Empty;
        await OnSubmit.InvokeAsync(value);
    }
}
