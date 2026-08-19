using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphSeriesPresentationTests
{
    [Fact]
    public void EightSeries_ReceiveEightDistinctPatternsInStableOrder()
    {
        var samples = Enumerable.Range(1, 8)
            .Select(index => new GraphExpressionSample(
                new GraphExpressionDefinition($"s{index}", $"Series {index}", $"x + {index}"),
                [new GraphSegment([new GraphPoint(0, index), new GraphPoint(1, index + 1)])],
                0))
            .ToArray();

        var presentations = GraphSeriesPresentationFactory.Create(samples);

        Assert.Equal(GraphSeriesLinePatternCatalog.PatternCount, presentations.Count);
        Assert.Equal(GraphSeriesLinePatternCatalog.PatternCount, presentations.Select(item => item.Pattern).Distinct().Count());
        Assert.Equal(GraphSeriesLinePattern.Solid, presentations[0].Pattern);
        Assert.Equal(GraphSeriesLinePattern.AlternatingDash, presentations[^1].Pattern);
        Assert.Equal("Series 1 [solid] — x + 1", presentations[0].LegendText);
    }

    [Fact]
    public void Presentation_PreservesSeriesCountsAndIdentity()
    {
        var sample = new GraphExpressionSample(
            new GraphExpressionDefinition("curve-a", "Curve A", "sin(x)"),
            [
                new GraphSegment([new GraphPoint(0, 0), new GraphPoint(1, 1)]),
                new GraphSegment([new GraphPoint(2, 2)])
            ],
            3);

        var presentation = GraphSeriesPresentationFactory.Create([sample]).Single();

        Assert.Equal("curve-a", presentation.Id);
        Assert.Equal("Curve A", presentation.Label);
        Assert.Equal("sin(x)", presentation.Expression);
        Assert.Equal(2, presentation.SegmentCount);
        Assert.Equal(3, presentation.ValidPointCount);
        Assert.Equal(3, presentation.InvalidSampleCount);
    }

    [Fact]
    public void MoreThanEightSeries_IsRejectedInsteadOfReusingAmbiguousPatterns()
    {
        var samples = Enumerable.Range(0, GraphSeriesLinePatternCatalog.PatternCount + 1)
            .Select(index => new GraphExpressionSample(
                new GraphExpressionDefinition($"s{index}", $"Series {index}", "x"),
                [],
                0))
            .ToArray();

        Assert.Throws<ArgumentOutOfRangeException>(() => GraphSeriesPresentationFactory.Create(samples));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    public void PatternForIndex_RejectsOutOfRangeIndexes(int index)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => GraphSeriesPresentationFactory.PatternForIndex(index));
    }
}
