using CalcNova.Graphing;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphSeriesPatternTests
{
    [Fact]
    public void FirstEightSeries_ReceiveDistinctPatterns()
    {
        var patterns = Enumerable.Range(0, GraphSeriesLinePatternCatalog.PatternCount)
            .Select(GraphSeriesLinePatternCatalog.ForSeriesIndex)
            .ToArray();

        Assert.Equal(GraphSeriesLinePatternCatalog.PatternCount, patterns.Distinct().Count());
    }

    [Fact]
    public void PatternAssignment_RepeatsDeterministicallyAfterCatalogBoundary()
    {
        Assert.Equal(
            GraphSeriesLinePatternCatalog.ForSeriesIndex(0),
            GraphSeriesLinePatternCatalog.ForSeriesIndex(GraphSeriesLinePatternCatalog.PatternCount));
        Assert.Equal(
            GraphSeriesLinePatternCatalog.ForSeriesIndex(3),
            GraphSeriesLinePatternCatalog.ForSeriesIndex(GraphSeriesLinePatternCatalog.PatternCount + 3));
    }

    [Fact]
    public void NonSolidPatterns_ContainVisibleAndSkippedEdges()
    {
        foreach (var pattern in Enum.GetValues<GraphSeriesLinePattern>().Where(pattern => pattern != GraphSeriesLinePattern.Solid))
        {
            var mask = Enumerable.Range(0, 40)
                .Select(index => GraphSeriesLinePatternCatalog.ShouldDrawEdge(pattern, index))
                .ToArray();

            Assert.Contains(true, mask);
            Assert.Contains(false, mask);
        }
    }

    [Fact]
    public void PatternMasks_AreDistinctWithinRepresentativeWindow()
    {
        var masks = Enum.GetValues<GraphSeriesLinePattern>()
            .Select(pattern => string.Concat(Enumerable.Range(0, 40)
                .Select(index => GraphSeriesLinePatternCatalog.ShouldDrawEdge(pattern, index) ? '1' : '0')))
            .ToArray();

        Assert.Equal(masks.Length, masks.Distinct(StringComparer.Ordinal).Count());
    }

    [Theory]
    [InlineData(GraphSeriesLinePattern.Solid, "solid")]
    [InlineData(GraphSeriesLinePattern.LongDash, "long dash")]
    [InlineData(GraphSeriesLinePattern.Dot, "dotted")]
    [InlineData(GraphSeriesLinePattern.DashDot, "dash-dot")]
    public void PatternLabels_AreHumanReadable(GraphSeriesLinePattern pattern, string expected)
    {
        Assert.Equal(expected, GraphSeriesLinePatternCatalog.GetLabel(pattern));
    }

    [Fact]
    public void NegativeSeriesIndex_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GraphSeriesLinePatternCatalog.ForSeriesIndex(-1));
    }
}
