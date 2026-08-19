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
    public string PatternLabel => GraphSeriesLinePatternCatalog.GetLabel(Pattern);

    public string LegendText => $"{Label} [{PatternLabel}] — {Expression}";
}

public static class GraphSeriesPresentationFactory
{
    public static IReadOnlyList<GraphSeriesPresentation> Create(IReadOnlyList<GraphExpressionSample> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);
        if (samples.Count > GraphSeriesLinePatternCatalog.PatternCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(samples),
                samples.Count,
                $"At most {GraphSeriesLinePatternCatalog.PatternCount} series can be presented distinctly.");
        }

        return samples
            .Select((sample, index) => new GraphSeriesPresentation(
                sample.Definition.Id,
                sample.Definition.Label,
                sample.Definition.Expression,
                GraphSeriesLinePatternCatalog.ForSeriesIndex(index),
                sample.Segments.Count,
                sample.ValidPointCount,
                sample.InvalidSampleCount))
            .ToArray();
    }

    public static GraphSeriesLinePattern PatternForIndex(int index)
    {
        if (index < 0 || index >= GraphSeriesLinePatternCatalog.PatternCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                $"Series index must be between 0 and {GraphSeriesLinePatternCatalog.PatternCount - 1}.");
        }

        return GraphSeriesLinePatternCatalog.ForSeriesIndex(index);
    }

    public static int DistinctPatternCount => GraphSeriesLinePatternCatalog.PatternCount;
}
