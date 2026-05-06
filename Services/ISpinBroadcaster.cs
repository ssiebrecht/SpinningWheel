using SpinningWheel.Models;

namespace SpinningWheel.Services;

/// <summary>
/// Decoupled pub/sub for spin lifecycle events so sibling organisms
/// (e.g. the Spotify widget) can react without being wired into the wheel.
/// </summary>
public interface ISpinBroadcaster
{
    event Action? SpinStarted;
    event Action<WheelEntry>? WinnerRevealed;

    void NotifySpinStarted();
    void NotifyWinnerRevealed(WheelEntry winner);
}
