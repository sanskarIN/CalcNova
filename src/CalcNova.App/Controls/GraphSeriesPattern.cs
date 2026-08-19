namespace CalcNova.App.Controls;

public enum GraphSeriesPattern
{
    Solid,
    LongDash,
    ShortDash,
    Dot,
    DashDot,
    SparseDash,
    DenseDash,
    AlternatingDash
}

public static class GraphSeriesPatternCatalog
{
    public const int PatternCount = 8;

    public static GraphSeriesPattern ForSeriesIndex(int seriesIndex)
    {
        if (seriesIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seriesIndex));
        }

        return (GraphSeriesPattern)(seriesIndex % PatternCount);
    }

    public static string GetLabel(GraphSeriesPattern pattern) => pattern switch
    {
        GraphSeriesPattern.Solid => "solid",
        GraphSeriesPattern.LongDash => "long dash",
        GraphSeriesPattern.ShortDash => "short dash",
        GraphSeriesPattern.Dot => "dotted",
        GraphSeriesPattern.DashDot => "dash-dot",
        GraphSeriesPattern.SparseDash => "sparse dash",
        GraphSeriesPattern.DenseDash => "dense dash",
        GraphSeriesPattern.AlternatingDash => "alternating dash",
        _ => throw new ArgumentOutOfRangeException(nameof(pattern))
    };

    public static bool ShouldDrawEdge(GraphSeriesPattern pattern, int edgeIndex)
    {
        if (edgeIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(edgeIndex));
        }

        return pattern switch
        {
            GraphSeriesPattern.Solid => true,
            GraphSeriesPattern.LongDash => edgeIndex % 8 < 5,
            GraphSeriesPattern.ShortDash => edgeIndex % 4 < 2,
            GraphSeriesPattern.Dot => edgeIndex % 3 == 0,
            GraphSeriesPattern.DashDot => edgeIndex % 10 is < 5 or 7,
            GraphSeriesPattern.SparseDash => edgeIndex % 9 < 3,
            GraphSeriesPattern.DenseDash => edgeIndex % 6 != 5,
            GraphSeriesPattern.AlternatingDash => edgeIndex % 5 is 0 or 1 or 3,
            _ => throw new ArgumentOutOfRangeException(nameof(pattern))
        };
    }
}
