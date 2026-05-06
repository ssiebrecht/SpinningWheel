namespace SpinningWheel.Models;

/// <summary>
/// Deterministic plan for one spin animation.
/// </summary>
/// <param name="WinnerIndex">Zero-based index of the winning entry.</param>
/// <param name="TargetRotationDeg">
/// Absolute rotation (accumulating; may be &gt; 360°) the wheel should animate to.
/// </param>
/// <param name="DurationMs">Duration of the CSS transition in milliseconds.</param>
public sealed record SpinPlan(int WinnerIndex, double TargetRotationDeg, int DurationMs);
