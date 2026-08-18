using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphSamplerTests
{
    private readonly GraphSampler _sampler = new();

    [Fact]
    public void Sample_SmoothFunction_ReturnsSingleSegment()
    {
        var result = _sampler.Sample("x ^ 2", new GraphSamplingOptions
        {
            MinimumX = -1d,
            MaximumX = 1d,
            SampleCount = 5
        });

        Assert.True(result.Success, result.ErrorMessage);
        var segment = Assert.Single(result.Segments);
        Assert.Equal(5, segment.Points.Count);
        Assert.Equal(1d, segment.Points[0].Y, 12);
        Assert.Equal(0d, segment.Points[2].Y, 12);
        Assert.Equal(1d, segment.Points[4].Y, 12);
        Assert.Equal(0, result.InvalidSampleCount);
    }

    [Fact]
    public void Sample_DivideByZero_BreaksGraphIntoSegments()
    {
        var result = _sampler.Sample("1 / x", new GraphSamplingOptions
        {
            MinimumX = -1d,
            MaximumX = 1d,
            SampleCount = 5
        });

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal(2, result.Segments.Count);
        Assert.Equal(1, result.InvalidSampleCount);
        Assert.All(result.Segments, segment => Assert.Equal(2, segment.Points.Count));
    }

    [Fact]
    public void Sample_PartialDomain_SkipsInvalidSamples()
    {
        var result = _sampler.Sample("sqrt(x)", new GraphSamplingOptions
        {
            MinimumX = -1d,
            MaximumX = 1d,
            SampleCount = 5
        });

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal(2, result.InvalidSampleCount);
        var segment = Assert.Single(result.Segments);
        Assert.Equal(3, segment.Points.Count);
        Assert.Equal(0d, segment.Points[0].X, 12);
    }

    [Fact]
    public void Sample_InvalidExpression_ReturnsFailure()
    {
        var result = _sampler.Sample("2 + * 3");

        Assert.False(result.Success);
        Assert.Empty(result.Segments);
        Assert.False(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [Fact]
    public void Sample_ExcessiveSampleCount_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _sampler.Sample("x", new GraphSamplingOptions
        {
            SampleCount = GraphSampler.MaximumSamples + 1
        }));
    }
}
