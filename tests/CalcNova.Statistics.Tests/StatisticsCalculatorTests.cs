using Xunit;

namespace CalcNova.Statistics.Tests;

public sealed class StatisticsCalculatorTests
{
    private readonly StatisticsCalculator _calculator = new();

    [Fact]
    public void Analyze_KnownDataset_ReturnsExpectedSummary()
    {
        var summary = _calculator.Analyze([1d, 2d, 2d, 3d, 4d]);

        Assert.Equal(5, summary.Count);
        Assert.Equal(12d, summary.Sum, 12);
        Assert.Equal(2.4d, summary.Mean, 12);
        Assert.Equal(2d, summary.Median, 12);
        Assert.Equal([2d], summary.Modes);
        Assert.Equal(1d, summary.Minimum, 12);
        Assert.Equal(4d, summary.Maximum, 12);
        Assert.Equal(3d, summary.Range, 12);
        Assert.Equal(1.04d, summary.PopulationVariance, 12);
        Assert.Equal(1.3d, summary.SampleVariance!.Value, 12);
        Assert.Equal(2d, summary.FirstQuartile, 12);
        Assert.Equal(3d, summary.ThirdQuartile, 12);
    }

    [Fact]
    public void Analyze_AllUniqueValues_ReturnsNoMode()
    {
        var summary = _calculator.Analyze([1d, 2d, 3d, 4d]);

        Assert.Empty(summary.Modes);
    }

    [Fact]
    public void Analyze_SingleValue_HasNoSampleVariance()
    {
        var summary = _calculator.Analyze([42d]);

        Assert.Equal(42d, summary.Mean, 12);
        Assert.Null(summary.SampleVariance);
        Assert.Null(summary.SampleStandardDeviation);
    }

    [Theory]
    [InlineData(0d, 10d)]
    [InlineData(0.25d, 17.5d)]
    [InlineData(0.5d, 25d)]
    [InlineData(0.75d, 32.5d)]
    [InlineData(1d, 40d)]
    public void Percentile_UsesLinearInterpolation(double percentile, double expected)
    {
        var value = _calculator.Percentile([10d, 20d, 30d, 40d], percentile);

        Assert.Equal(expected, value, 12);
    }

    [Fact]
    public void Analyze_RejectsNonFiniteValues()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Analyze([1d, double.NaN]));
    }
}
