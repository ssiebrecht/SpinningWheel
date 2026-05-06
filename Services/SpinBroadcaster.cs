using SpinningWheel.Models;

namespace SpinningWheel.Services;

/// <inheritdoc />
public sealed class SpinBroadcaster : ISpinBroadcaster
{
    public event Action? SpinStarted;
    public event Action<WheelEntry>? WinnerRevealed;

    public void NotifySpinStarted() => SpinStarted?.Invoke();

    public void NotifyWinnerRevealed(WheelEntry winner) => WinnerRevealed?.Invoke(winner);
}
