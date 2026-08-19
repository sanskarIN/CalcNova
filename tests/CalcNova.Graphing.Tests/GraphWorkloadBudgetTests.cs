using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphWorkloadBudgetTests
{
    [Fact]
    public void GraphSampler_RejectsSampleCountAboveHardBudget()
    {
        var sampler = new GraphSampler();
        var options = new GraphSamplingOptions
        {
            SampleCount = GraphSampler.MaximumSamples + 1
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => sampler.Sample("x", options));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10001)]
    public void NumericalOptions_RejectRootIterationBudgetOutsideSupportedRange(int iterations)
    {
        var options = new NumericalAnalysisOptions
        {
            MaximumRootIterations = iterations
        };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Fact]
    public void NumericalOptions_RejectIntegrationIntervalsAboveConfiguredMaximum()
    {
        var options = new NumericalAnalysisOptions
        {
            MaximumIntegrationIntervals = 100,
            IntegrationIntervals = 102
        };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Fact]
    public void NumericalOptions_RejectMaximumIntegrationBudgetAboveHardCap()
    {
        var options = new NumericalAnalysisOptions
        {
            MaximumIntegrationIntervals = 1_000_001
        };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Theory]
    [InlineData(0d)]
    [InlineData(-1d)]
    [InlineData(double.PositiveInfinity)]
    public void GraphSampler_RejectsInvalidMaximumAbsoluteY(double maximumAbsoluteY)
    {
        var sampler = new GraphSampler();
        var options = new GraphSamplingOptions
        {
            MaximumAbsoluteY = maximumAbsoluteY
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => sampler.Sample("x", options));
    }

    [Theory]
    [InlineData(0d)]
    [InlineData(-1d)]
    [InlineData(double.NaN)]
    public void GraphSampler_RejectsInvalidDiscontinuityThreshold(double threshold)
    {
        var sampler = new GraphSampler();
        var options = new GraphSamplingOptions
        {
            DiscontinuityJumpThreshold = threshold
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => sampler.Sample("x", options));
    }
}
