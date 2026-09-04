// CalcNova.Graphing/GraphSampler.cs
using System;
using System.Collections.Generic;

namespace CalcNova.Graphing;

public static class GraphSampler
{
    /// <summary>
    /// Samples an expression across the viewport and partitions continuous curves
    /// to avoid drawing artifacts across asymptotes and undefined points.
    /// </summary>
    public static MultiGraphSamplingResult SampleExpression(
        Func<double, double> evaluator,
        GraphViewport viewport,
        GraphSamplingOptions options)
    {
        var segments = new List<GraphSegment>();
        var currentSegmentPoints = new List<GraphPoint>();

        double step = (viewport.XMax - viewport.XMin) / options.Resolution;
        double viewportHeight = Math.Abs(viewport.YMax - viewport.YMin);
        
        // Threshold: A jump exceeding 3x the visible viewport height
        // combined with a sign change indicates a vertical asymptote.
        double asymptoticJumpThreshold = viewportHeight * 3.0;

        for (double x = viewport.XMin; x <= viewport.XMax; x += step)
        {
            double y;
            try
            {
                y = evaluator(x);
            }
            catch
            {
                y = double.NaN;
            }

            // 1. Check for non-finite evaluations (NaN, Infinity)
            if (double.IsNaN(y) || double.IsInfinity(y))
            {
                FlushSegment(segments, currentSegmentPoints);
                continue;
            }

            // 2. Check for asymptotic discontinuity between adjacent points
            if (currentSegmentPoints.Count > 0)
            {
                var prev = currentSegmentPoints[^1];
                bool oppositeSigns = (prev.Y > 0 && y < 0) || (prev.Y < 0 && y > 0);
                double deltaY = Math.Abs(y - prev.Y);

                if (oppositeSigns && deltaY > asymptoticJumpThreshold)
                {
                    FlushSegment(segments, currentSegmentPoints);
                }
            }

            currentSegmentPoints.Add(new GraphPoint(x, y));
        }

        FlushSegment(segments, currentSegmentPoints);

        return new MultiGraphSamplingResult(segments);
    }

    private static void FlushSegment(List<GraphSegment> target, List<GraphPoint> current)
    {
        if (current.Count > 0)
        {
            target.Add(new GraphSegment(new List<GraphPoint>(current)));
            current.Clear();
        }
    }
}
