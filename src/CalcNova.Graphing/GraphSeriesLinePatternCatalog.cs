namespace CalcNova.Graphing;

public static class GraphSeriesLinePatternCatalog
{
    private static readonly GraphSeriesLinePattern[] Patterns = Enum.GetValues<GraphSeriesLinePattern>();

    public static int PatternCount => Patterns.Length;

    public static GraphSeriesLinePattern ForSeriesIndex(int seriesIndex)
    {
        if (seriesIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));
        }

        return Patterns[seriesIndex % Patterns.Length];
    }

    public static string GetLabel(GraphSeriesLinePattern pattern) => pattern switch
    {
        GraphSeriesLinePattern.Solid => "solid",
        GraphSeriesLinePattern.LongDash => "long dash",
        GraphSeriesLinePattern.ShortDash => "short dash",
        GraphSeriesLinePattern.Dot => "dotted",
        GraphSeriesLinePattern.DashDot => "dash-dot",
        GraphSeriesLinePattern.SparseDash => "sparse dash",
        GraphSeriesLinePattern.DenseDash => "dense dash",
        GraphSeriesLinePattern.AlternatingDash => "alternating dash",
        _ => throw new ArgumentOutOfRangeException(nameof(pattern))
    };

    public static bool ShouldDrawEdge(GraphSeriesLinePattern pattern, int edgeIndex)
    {
        if (edgeIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(edgeIndex));
        }

        return pattern switch
        {
            GraphSeriesLinePattern.Solid => true,
            GraphSeriesLinePattern.LongDash => edgeIndex % 8 < 5,
            GraphSeriesLinePattern.ShortDash => edgeIndex % 4 < 2,
            GraphSeriesLinePattern.Dot => edgeIndex % 3 == 0,
            GraphSeriesLinePattern.DashDot => edgeIndex % 10 is < 5 or 7,
            GraphSeriesLinePattern.SparseDash => edgeIndex % 9 < 3,
            GraphSeriesLinePattern.DenseDash => edgeIndex % 6 != 5,
            GraphSeriesLinePattern.AlternatingDash => edgeIndex % 5 is 0 or 1 or 3,
            _ => throw new ArgumentOutOfRangeException(nameof(pattern))
        };
    }
}
