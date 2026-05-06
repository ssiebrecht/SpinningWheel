using SpinningWheel.Models;

namespace SpinningWheel.Services;

/// <inheritdoc />
public sealed class SpinService : ISpinService
{
    private const int MinFullTurns = 5;
    private const int DurationMs = 5200;
    private const int StopFullTurns = 2;

    /// <summary>
    /// Initial slope of the CSS easing <c>cubic-bezier(0.17, 0.67, 0.24, 1)</c>.
    /// Slope = P1.y / P1.x = 0.67 / 0.17 ≈ 3.9412.
    /// Used to compute the deceleration duration so the animation starts
    /// at exactly the current angular velocity.
    /// </summary>
    private const double EasingInitialSlope = 0.67 / 0.17;

    private const int StopDurationMinMs = 2_000;
    private const int StopDurationMaxMs = 8_000;

    /// <summary>
    /// The angle (in degrees, clockwise from top) where the pointer/stopper is positioned.
    /// 0 = top, 90 = right, 180 = bottom, 270 = left.
    /// </summary>
    private const double PointerAngleDeg = 90.0;

    private readonly Random _random = new();

    public SpinPlan Plan(int segmentCount, double currentRotationDeg)
    {
        var (winnerIndex, totalDeg) = ComputeWinner(segmentCount, currentRotationDeg, MinFullTurns);
        var target = currentRotationDeg + totalDeg;
        return new SpinPlan(winnerIndex, target, DurationMs);
    }

    public SpinPlan PlanStop(int segmentCount, double currentRotationDeg, double angularVelocityDegPerMs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(angularVelocityDegPerMs, 0);

        var (winnerIndex, totalDeg) = ComputeWinner(segmentCount, currentRotationDeg, StopFullTurns);

        // duration = slope × totalDeg / velocity  →  initial animation speed ≈ velocity.
        var durationMs = (int)Math.Ceiling(EasingInitialSlope * totalDeg / angularVelocityDegPerMs);
        durationMs = Math.Clamp(durationMs, StopDurationMinMs, StopDurationMaxMs);

        var target = currentRotationDeg + totalDeg;
        return new SpinPlan(winnerIndex, target, durationMs);
    }

    /// <summary>
    /// Picks a random winner and computes the total rotation (in degrees) needed
    /// to land the winner under the pointer.
    /// </summary>
    private (int WinnerIndex, double TotalDeg) ComputeWinner(
        int segmentCount, double currentRotationDeg, int fullTurns)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(segmentCount, 2);

        var step = 360.0 / segmentCount;
        var winnerIndex = _random.Next(segmentCount);

        // Angle (clockwise from top) of the winning segment's centre in the wheel's own frame.
        var winnerCentre = (winnerIndex + 0.5) * step;

        // We want (currentRotation + delta) mod 360 == (360 - PointerAngleDeg - winnerCentre) mod 360,
        // so the winner lands under the pointer at the right.
        var desiredMod = Mod360(360.0 - PointerAngleDeg - winnerCentre);
        var currentMod = Mod360(currentRotationDeg);
        var delta = desiredMod - currentMod;
        if (delta <= 0)
        {
            delta += 360;
        }

        // Jitter within ±40% of half a segment so the pointer never lands exactly on a seam.
        var jitterRange = step * 0.4;
        var jitter = (_random.NextDouble() * 2.0 - 1.0) * jitterRange;

        return (winnerIndex, fullTurns * 360.0 + delta + jitter);
    }

    private static double Mod360(double value)
    {
        var r = value % 360.0;
        return r < 0 ? r + 360.0 : r;
    }
}
