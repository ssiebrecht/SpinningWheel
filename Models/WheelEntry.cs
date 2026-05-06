namespace SpinningWheel.Models;

/// <summary>
/// Represents a single entry on the wheel.
/// </summary>
/// <param name="Id">Stable id.</param>
/// <param name="Name">Display name.</param>
/// <param name="ColorToken">Name of a CSS custom property (e.g. <c>--color-segment-1</c>).</param>
public sealed record WheelEntry(Guid Id, string Name, string ColorToken);
