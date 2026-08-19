using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphTraceLocatorTests
{
    [Fact]
    public void FindNearest_ReturnsClosestSampledPoint()
    {
        var sampler = new GraphSampler();
        var sample = sampler.Sample("x ^ 2", new GraphSamplingOptions
        {
            MinimumX = 0,
            MaximumX = 4,
            SampleCount = 5
        });
        Assert.True(sample.Success);

        var trace = GraphTraceLocator.FindNearest(sample.Segments, 2.2);

        Assert.Equal(1, trace.Segment);
        Assert.Equal(2, trace.X, 12);
        Assert.Equal(4, trace.Y, 12);
        Assert.Equal(0.2, trace.Distance, 12);
    }

    [Fact]
    public void FindNearest_PreservesSegmentNumberAcrossDiscontinuity()
    {
        var sampler = new GraphSampler();
        var sample = sampler.Sample("1 / x", new GraphSamplingOptions
        {
            MinimumX = -1,
            MaximumX = 1,
            SampleCount = 5
        });
        Assert.True(sample.Success);
        Assert.True(sample.Segments.Count >= 2);

        var trace = GraphTraceLocator.FindNearest(sample.Segments, 0.7);

        Assert.True(trace.Segment >= 2);
        Assert.True(trace.X > 0);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void FindNearest_RejectsNonFiniteRequestedX(double requestedX)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            GraphTraceLocator.FindNearest(Array.Empty<GraphSegment>(), requestedX));
    }

    [Fact]
    public void FindNearest_RejectsEmptySampleSet()
    {
        Assert.Throws<InvalidOperationException>(() =>
            GraphTraceLocator.FindNearest(Array.Empty<GraphSegment>(), 0));
    }
}
