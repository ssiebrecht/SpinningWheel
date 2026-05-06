using SpinningWheel.Models;

namespace SpinningWheel.Services;

/// <summary>
/// Plans spin animations so the winner is deterministic before the animation starts.
/// </summary>
public interface ISpinService
{
    /// <param name="segmentCount">Number of segments on the wheel (&gt;= 2).</param>
    /// <param name="currentRotationDeg">
    /// Current accumulated rotation of the wheel in degrees (may be arbitrarily large).
    /// </param>
    SpinPlan Plan(int segmentCount, double currentRotationDeg);

    /// <summary>
    /// Plans a deceleration spin that smoothly stops the wheel on a randomly chosen winner.
    /// The duration is calculated so the animation's initial speed matches
    /// <paramref name="angularVelocityDegPerMs"/> – no visible speed jump.
    /// </summary>
    /// <param name="segmentCount">Number of segments on the wheel (&gt;= 2).</param>
    /// <param name="currentRotationDeg">
    /// Current visual rotation of the wheel in degrees at the moment the user clicks to stop.
    /// </param>
    /// <param name="angularVelocityDegPerMs">
    /// Current angular velocity of the wheel in degrees per millisecond.
    /// </param>
    SpinPlan PlanStop(int segmentCount, double currentRotationDeg, double angularVelocityDegPerMs);
}
