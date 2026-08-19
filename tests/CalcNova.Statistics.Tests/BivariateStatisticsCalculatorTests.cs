using CalcNova.Statistics;
using Xunit;

namespace CalcNova.Statistics.Tests;

public sealed class BivariateStatisticsCalculatorTests
{
    private readonly BivariateStatisticsCalculator _calculator = new();

    [Fact]
    public void Analyze_PerfectPositiveRelationship_ComputesRegressionAndCorrelation()
    {
        var summary = _calculator.Analyze([1d, 2d, 3d, 4d], [3d, 5d, 7d, 9d]);

        Assert.Equal(4, summary.Count);
        Assert.Equal(2.5d, summary.MeanX, 12);
        Assert.Equal(6d, summary.MeanY, 12);
        Assert.Equal(2.5d, summary.PopulationCovariance, 12);
        Assert.Equal(10d / 3d, summary.SampleCovariance!.Value, 12);
        Assert.Equal(1d, summary.PearsonCorrelation!.Value, 12);
        Assert.Equal(2d, summary.RegressionSlope!.Value, 12);
        Assert.Equal(1d, summary.RegressionIntercept!.Value, 12);
        Assert.Equal(1d, summary.RSquared!.Value, 12);
        Assert.True(summary.HasLinearRegression);
        Assert.Equal(11d, summary.Predict(5d), 12);
    }

    [Fact]
    public void Analyze_PerfectNegativeRelationship_ReportsNegativeCorrelation()
    {
        var summary = _calculator.Analyze([1d, 2d, 3d], [6d, 4d, 2d]);

        Assert.Equal(-1d, summary.PearsonCorrelation!.Value, 12);
        Assert.Equal(-2d, summary.RegressionSlope!.Value, 12);
        Assert.Equal(8d, summary.RegressionIntercept!.Value, 12);
        Assert.Equal(1d, summary.RSquared!.Value, 12);
    }

    [Fact]
    public void Analyze_ConstantY_ProducesZeroSlopeButUndefinedCorrelation()
    {
        var summary = _calculator.Analyze([1d, 2d, 3d], [5d, 5d, 5d]);

        Assert.Null(summary.PearsonCorrelation);
        Assert.Equal(0d, summary.RegressionSlope!.Value, 12);
        Assert.Equal(5d, summary.RegressionIntercept!.Value, 12);
        Assert.Null(summary.RSquared);
        Assert.Equal(5d, summary.Predict(100d), 12);
    }

    [Fact]
    public void Analyze_ConstantX_MakesLinearRegressionUndefined()
    {
        var summary = _calculator.Analyze([2d, 2d, 2d], [1d, 2d, 3d]);

        Assert.Null(summary.PearsonCorrelation);
        Assert.Null(summary.RegressionSlope);
        Assert.Null(summary.RegressionIntercept);
        Assert.Null(summary.RSquared);
        Assert.False(summary.HasLinearRegression);
        Assert.Throws<InvalidOperationException>(() => summary.Predict(4d));
    }

    [Fact]
    public void Analyze_SinglePair_HasPopulationCovarianceButNoSampleCovariance()
    {
        var summary = _calculator.Analyze([4d], [9d]);

        Assert.Equal(0d, summary.PopulationCovariance);
        Assert.Null(summary.SampleCovariance);
        Assert.Null(summary.PearsonCorrelation);
        Assert.Null(summary.RegressionSlope);
    }

    [Fact]
    public void Analyze_RejectsUnequalDatasetLengths()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Analyze([1d, 2d], [3d]));
    }

    [Fact]
    public void Analyze_RejectsEmptyDatasets()
    {
        Assert.Throws<ArgumentException>(() => _calculator.Analyze(Array.Empty<double>(), Array.Empty<double>()));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Analyze_RejectsNonFiniteValues(double invalid)
    {
        Assert.Throws<ArgumentException>(() => _calculator.Analyze([1d, invalid], [2d, 3d]));
        Assert.Throws<ArgumentException>(() => _calculator.Analyze([1d, 2d], [3d, invalid]));
    }

    [Fact]
    public void Analyze_RejectsPairsAboveHardWorkloadBudget()
    {
        var x = Enumerable.Repeat(1d, BivariateStatisticsCalculator.MaximumPairCount + 1);
        var y = Enumerable.Repeat(2d, BivariateStatisticsCalculator.MaximumPairCount + 1);

        Assert.Throws<ArgumentException>(() => _calculator.Analyze(x, y));
    }

    [Fact]
    public void Predict_RejectsNonFiniteInput()
    {
        var summary = _calculator.Analyze([1d, 2d], [2d, 4d]);

        Assert.Throws<ArgumentOutOfRangeException>(() => summary.Predict(double.NaN));
    }
}
