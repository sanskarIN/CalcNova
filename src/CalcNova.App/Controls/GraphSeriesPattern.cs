using CalcNova.Graphing;

namespace CalcNova.App.Controls;

public enum GraphSeriesPattern
{
    Solid = 0,
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
    public static int PatternCount => GraphSeriesLinePatternCatalog.PatternCount;

    public static GraphSeriesPattern ForSeriesIndex(int seriesIndex) =>
        (GraphSeriesPattern)GraphSeriesLinePatternCatalog.ForSeriesIndex(seriesIndex);

    public static string GetLabel(GraphSeriesPattern pattern) =>
        GraphSeriesLinePatternCatalog.GetLabel((GraphSeriesLinePattern)pattern);

    public static bool ShouldDrawEdge(GraphSeriesPattern pattern, int edgeIndex) =>
        GraphSeriesLinePatternCatalog.ShouldDrawEdge((GraphSeriesLinePattern)pattern, edgeIndex);
}
