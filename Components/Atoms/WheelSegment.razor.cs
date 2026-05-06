using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace SpinningWheel.Components.Atoms;

public partial class WheelSegment
{
    [Parameter, EditorRequired] public double StartAngle { get; set; }
    [Parameter, EditorRequired] public double EndAngle { get; set; }
    [Parameter, EditorRequired] public double Radius { get; set; }
    [Parameter, EditorRequired] public string Label { get; set; } = string.Empty;
    [Parameter, EditorRequired] public string ColorToken { get; set; } = "--color-segment-1";

    private string PathData => BuildPath();
    private string LabelTransform => BuildLabelTransform();

    private string BuildPath()
    {
        var sweep = EndAngle - StartAngle;

        // Special case: a single segment spanning the full circle (360°).
        // SVG arcs with coincident start/end points render nothing, so we
        // draw two half-circle arcs instead.
        if (sweep >= 359.99)
        {
            var (topX, topY) = Polar(StartAngle, Radius);
            var (bottomX, bottomY) = Polar(StartAngle + 180, Radius);
            return string.Create(CultureInfo.InvariantCulture,
                $"M {topX:F3} {topY:F3} A {Radius:F3} {Radius:F3} 0 1 1 {bottomX:F3} {bottomY:F3} A {Radius:F3} {Radius:F3} 0 1 1 {topX:F3} {topY:F3} Z");
        }

        var (sx, sy) = Polar(StartAngle, Radius);
        var (ex, ey) = Polar(EndAngle, Radius);
        var largeArc = sweep > 180 ? 1 : 0;

        return string.Create(CultureInfo.InvariantCulture,
            $"M 0 0 L {sx:F3} {sy:F3} A {Radius:F3} {Radius:F3} 0 {largeArc} 1 {ex:F3} {ey:F3} Z");
    }

    private string BuildLabelTransform()
    {
        var centre = (StartAngle + EndAngle) / 2.0;
        var distance = Radius * 0.62;

        // Rotate to the segment's centre, move outward along the radius,
        // then rotate -90° so the text reads from the hub toward the rim.
        return string.Create(CultureInfo.InvariantCulture,
            $"rotate({centre:F3}) translate(0 {-distance:F3}) rotate(-90)");
    }

    /// <summary>
    /// Converts a polar angle (degrees, clockwise from the top/12-o'clock)
    /// into SVG Cartesian coordinates (y-axis down).
    /// </summary>
    private static (double X, double Y) Polar(double angleDeg, double r)
    {
        var rad = angleDeg * Math.PI / 180.0;
        return (r * Math.Sin(rad), -r * Math.Cos(rad));
    }
}
