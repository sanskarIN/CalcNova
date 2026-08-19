namespace CalcNova.Graphing;

public sealed record GraphSeriesPresentation(
    string Id,
    string Label,
    string Expression,
    GraphSeriesLinePattern Pattern,
    int SegmentCount,
    int ValidPointCount,
    int InvalidSampleCount)
{
    public string PatternLabel => Pattern switch
    {
        GraphSeriesLinePattern.Solid => "solid",
        GraphSeriesLinePattern.Dash => "dashed",
        GraphSeriesLinePattern.Dot => "dotted",
        GraphSeriesLinePattern.DashDot => "dash-dot",
        GraphSeriesLinePattern.DashDotDot => "dash-dot-dot",
        GraphSeriesLinePattern.LongDash => "long dash",
        GraphSeriesLinePattern.ShortDash => "short dash",
        GraphSeriesLinePattern.SparseDot => "sparse dot",
        _ => "line"
    };

    public string LegendText => $"{Label} [{PatternLabel}] — {Expression}";
}

public static class GraphSeriesPresentationFactory
{
    private static readonly GraphSeriesLinePattern[] Patterns =
    [
        GraphSeriesLinePattern.Solid,
        GraphSeriesLinePattern.Dash,
        GraphSeriesLinePattern.Dot,
        GraphSeriesLinePattern.DashDot,
        GraphSeriesLinePattern.DashDotDot,
        GraphSeriesLinePattern.LongDash,
        GraphSeriesLinePattern.ShortDash,
        GraphSeriesLinePattern.SparseDot
    ];

    public static IReadOnlyList<GraphSeriesPresentation> Create(IReadOnlyList<GraphExpressionSample> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);
        if (samples.Count > Patterns.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(samples), samples.Count, $"At most {Patterns.Length} series can be presented distinctly.");
        }

        return samples
            .Select((sample, index) => new GraphSeriesPresentation(
                sample.Definition.Id,
                sample.Definition.Label,
                sample.Definition.Expression,
                Patterns[index],
                sample.Segments.Count,
                sample.ValidPointCount,
                sample.InvalidSampleCount))
            .ToArray();
    }

    public static GraphSeriesLinePattern PatternForIndex(int index)
    {
        if (index < 0 || index >= Patterns.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Series index must be between 0 and {Patterns.Length - 1}.");
        }

        return Patterns[index];
    }

    public static int DistinctPatternCount => Patterns.Length;
}
