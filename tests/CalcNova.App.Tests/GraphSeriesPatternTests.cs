using CalcNova.App.Controls;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphSeriesPatternTests
{
    [Fact]
    public void FirstEightSeries_ReceiveDistinctPatterns()
    {
        var patterns = Enumerable.Range(0, GraphSeriesPatternCatalog.PatternCount)
            .Select(GraphSeriesPatternCatalog.ForSeriesIndex)
            .ToArray();

        Assert.Equal(GraphSeriesPatternCatalog.PatternCount, patterns.Distinct().Count());
    }

    [Fact]
    public void PatternAssignment_RepeatsDeterministicallyAfterCatalogBoundary()
    {
        Assert.Equal(
            GraphSeriesPatternCatalog.ForSeriesIndex(0),
            GraphSeriesPatternCatalog.ForSeriesIndex(GraphSeriesPatternCatalog.PatternCount));
        Assert.Equal(
            GraphSeriesPatternCatalog.ForSeriesIndex(3),
            GraphSeriesPatternCatalog.ForSeriesIndex(GraphSeriesPatternCatalog.PatternCount + 3));
    }

    [Fact]
    public void NonSolidPatterns_ContainVisibleAndSkippedEdges()
    {
        foreach (var pattern in Enum.GetValues<GraphSeriesPattern>().Where(pattern => pattern != GraphSeriesPattern.Solid))
        {
            var mask = Enumerable.Range(0, 40)
                .Select(index => GraphSeriesPatternCatalog.ShouldDrawEdge(pattern, index))
                .ToArray();

            Assert.Contains(true, mask);
            Assert.Contains(false, mask);
        }
    }

    [Fact]
    public void PatternMasks_AreDistinctWithinRepresentativeWindow()
    {
        var masks = Enum.GetValues<GraphSeriesPattern>()
            .Select(pattern => string.Concat(Enumerable.Range(0, 40)
                .Select(index => GraphSeriesPatternCatalog.ShouldDrawEdge(pattern, index) ? '1' : '0')))
            .ToArray();

        Assert.Equal(masks.Length, masks.Distinct(StringComparer.Ordinal).Count());
    }

    [Theory]
    [InlineData(GraphSeriesPattern.Solid, "solid")]
    [InlineData(GraphSeriesPattern.LongDash, "long dash")]
    [InlineData(GraphSeriesPattern.Dot, "dotted")]
    [InlineData(GraphSeriesPattern.DashDot, "dash-dot")]
    public void PatternLabels_AreHumanReadable(GraphSeriesPattern pattern, string expected)
    {
        Assert.Equal(expected, GraphSeriesPatternCatalog.GetLabel(pattern));
    }

    [Fact]
    public void NegativeSeriesIndex_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GraphSeriesPatternCatalog.ForSeriesIndex(-1));
    }
}
