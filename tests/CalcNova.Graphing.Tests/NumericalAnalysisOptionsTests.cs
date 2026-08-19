using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class NumericalAnalysisOptionsTests
{
    [Theory]
    [InlineData(0d)]
    [InlineData(-1d)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void DerivativeStep_MustBeFiniteAndPositive(double step)
    {
        var options = new NumericalAnalysisOptions { DerivativeStep = step };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Theory]
    [InlineData(0d)]
    [InlineData(-1d)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void RootTolerance_MustBeFiniteAndPositive(double tolerance)
    {
        var options = new NumericalAnalysisOptions { RootTolerance = tolerance };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10001)]
    public void RootIterationLimit_IsBounded(int iterations)
    {
        var options = new NumericalAnalysisOptions { MaximumRootIterations = iterations };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1000001)]
    public void MaximumIntegrationIntervals_IsBounded(int maximum)
    {
        var options = new NumericalAnalysisOptions { MaximumIntegrationIntervals = maximum };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(99999)]
    public void IntegrationIntervals_MustBeEvenAndWithinConfiguredMaximum(int intervals)
    {
        var options = new NumericalAnalysisOptions
        {
            IntegrationIntervals = intervals,
            MaximumIntegrationIntervals = 100000
        };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Fact]
    public void IntegrationIntervals_CannotExceedConfiguredMaximum()
    {
        var options = new NumericalAnalysisOptions
        {
            IntegrationIntervals = 102,
            MaximumIntegrationIntervals = 100
        };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }

    [Fact]
    public void BoundaryConfiguration_IsAccepted()
    {
        var options = new NumericalAnalysisOptions
        {
            DerivativeStep = double.Epsilon,
            RootTolerance = double.Epsilon,
            MaximumRootIterations = 10000,
            IntegrationIntervals = 1000000,
            MaximumIntegrationIntervals = 1000000
        };

        options.Validate();
    }
}
