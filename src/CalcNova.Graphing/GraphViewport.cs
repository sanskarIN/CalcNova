namespace CalcNova.Graphing;

public readonly record struct GraphViewport(double MinimumX, double MaximumX, double MinimumY, double MaximumY)
{
    public double Width => MaximumX - MinimumX;

    public double Height => MaximumY - MinimumY;

    public void Validate()
    {
        if (!double.IsFinite(MinimumX) ||
            !double.IsFinite(MaximumX) ||
            !double.IsFinite(MinimumY) ||
            !double.IsFinite(MaximumY) ||
            MinimumX >= MaximumX ||
            MinimumY >= MaximumY)
        {
            throw new ArgumentException("Graph viewport bounds must be finite and strictly increasing.");
        }
    }

    public static GraphViewport FromSegments(IReadOnlyList<GraphSegment> segments, double paddingRatio = 0.08d)
    {
        ArgumentNullException.ThrowIfNull(segments);
        if (!double.IsFinite(paddingRatio) || paddingRatio is < 0d or > 1d)
        {
            throw new ArgumentOutOfRangeException(nameof(paddingRatio), paddingRatio, "Padding ratio must be between 0 and 1.");
        }

        var points = segments
            .SelectMany(segment => segment.Points)
            .Where(point => double.IsFinite(point.X) && double.IsFinite(point.Y))
            .ToArray();

        if (points.Length == 0)
        {
            return new GraphViewport(-10d, 10d, -10d, 10d);
        }

        var minimumX = points.Min(point => point.X);
        var maximumX = points.Max(point => point.X);
        var minimumY = points.Min(point => point.Y);
        var maximumY = points.Max(point => point.Y);
        Normalize(ref minimumX, ref maximumX);
        Normalize(ref minimumY, ref maximumY);

        var horizontalPadding = (maximumX - minimumX) * paddingRatio;
        var verticalPadding = (maximumY - minimumY) * paddingRatio;
        return new GraphViewport(
            minimumX - horizontalPadding,
            maximumX + horizontalPadding,
            minimumY - verticalPadding,
            maximumY + verticalPadding);
    }

    private static void Normalize(ref double minimum, ref double maximum)
    {
        if (minimum == maximum)
        {
            var expansion = Math.Max(1d, Math.Abs(minimum) * 0.1d);
            minimum -= expansion;
            maximum += expansion;
        }
    }
}
