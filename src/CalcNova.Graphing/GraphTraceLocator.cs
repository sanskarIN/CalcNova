namespace CalcNova.Graphing;

public static class GraphTraceLocator
{
    public static GraphTraceResult FindNearest(
        IEnumerable<GraphSegment> segments,
        double requestedX)
    {
        ArgumentNullException.ThrowIfNull(segments);
        if (!double.IsFinite(requestedX))
        {
            throw new ArgumentOutOfRangeException(nameof(requestedX), requestedX, "Trace X must be finite.");
        }

        GraphTraceResult? best = null;
        var segmentIndex = 0;
        foreach (var segment in segments)
        {
            segmentIndex++;
            foreach (var point in segment.Points)
            {
                if (!double.IsFinite(point.X) || !double.IsFinite(point.Y))
                {
                    continue;
                }

                var candidate = new GraphTraceResult(segmentIndex, requestedX, point.X, point.Y);
                if (best is null || candidate.Distance < best.Distance)
                {
                    best = candidate;
                }
            }
        }

        return best ?? throw new InvalidOperationException("No valid sampled graph point is available for tracing.");
    }
}
