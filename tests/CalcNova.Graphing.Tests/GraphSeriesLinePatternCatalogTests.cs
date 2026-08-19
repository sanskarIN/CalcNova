using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphSeriesLinePatternCatalogTests
{
    [Fact]
    public void Catalog_ContainsEightDistinctPatterns()
    {
        var patterns = Enumerable.Range(0, GraphSeriesLinePatternCatalog.PatternCount)
            .Select(GraphSeriesLinePatternCatalog.ForSeriesIndex)
            .ToArray();

        Assert.Equal(8, GraphSeriesLinePatternCatalog.PatternCount);
        Assert.Equal(8, patterns.Distinct().Count());
    }

    [Fact]
    public void EveryNonSolidPattern_HasVisibleAndHiddenEdges()
    {
        foreach (var pattern in Enum.GetValues<GraphSeriesLinePattern>())
        {
            var mask = Enumerable.Range(0, 80)
                .Select(index => GraphSeriesLinePatternCatalog.ShouldDrawEdge(pattern, index))
                .ToArray();

            Assert.Contains(true, mask);
            if (pattern != GraphSeriesLinePattern.Solid)
            {
                Assert.Contains(false, mask);
            }
        }
    }

    [Fact]
    public void PatternMasks_AreUniqueAcrossRepresentativeWindow()
    {
        var masks = Enum.GetValues<GraphSeriesLinePattern>()
            .Select(pattern => string.Concat(Enumerable.Range(0, 80)
                .Select(index => GraphSeriesLinePatternCatalog.ShouldDrawEdge(pattern, index) ? '1' : '0')))
            .ToArray();

        Assert.Equal(masks.Length, masks.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void PatternLabels_AreNonEmptyAndUnique()
    {
        var labels = Enum.GetValues<GraphSeriesLinePattern>()
            .Select(GraphSeriesLinePatternCatalog.GetLabel)
            .ToArray();

        Assert.All(labels, label => Assert.False(string.IsNullOrWhiteSpace(label)));
        Assert.Equal(labels.Length, labels.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void PatternAssignment_RepeatsOnlyAfterFullCatalog()
    {
        Assert.Equal(
            GraphSeriesLinePattern.Solid,
            GraphSeriesLinePatternCatalog.ForSeriesIndex(GraphSeriesLinePatternCatalog.PatternCount));
    }

    [Fact]
    public void NegativeIndexes_AreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GraphSeriesLinePatternCatalog.ForSeriesIndex(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => GraphSeriesLinePatternCatalog.ShouldDrawEdge(GraphSeriesLinePattern.Solid, -1));
    }
}
