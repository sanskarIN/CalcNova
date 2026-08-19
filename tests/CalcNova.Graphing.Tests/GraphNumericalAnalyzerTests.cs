using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphNumericalAnalyzerTests
{
    private readonly GraphNumericalAnalyzer _analyzer = new();

    [Fact]
    public void Derivative_ApproximatesPolynomialSlope()
    {
        var derivative = _analyzer.Derivative("x ^ 2", 3d);

        Assert.Equal(6d, derivative, 6);
    }

    [Fact]
    public void FindRoot_FindsBracketedPolynomialRoot()
    {
        var root = _analyzer.FindRoot("x ^ 2 - 2", 1d, 2d);

        Assert.Equal(Math.Sqrt(2d), root, 8);
    }

    [Fact]
    public void FindRoot_RejectsIntervalWithoutSignChange()
    {
        Assert.Throws<InvalidOperationException>(() => _analyzer.FindRoot("x ^ 2 + 1", -1d, 1d));
    }

    [Fact]
    public void Integrate_ApproximatesPolynomialArea()
    {
        var integral = _analyzer.Integrate("x ^ 2", 0d, 3d);

        Assert.Equal(9d, integral, 8);
    }

    [Fact]
    public void Integrate_ReversedBoundsNegateResult()
    {
        var forward = _analyzer.Integrate("x", 0d, 2d);
        var reversed = _analyzer.Integrate("x", 2d, 0d);

        Assert.Equal(-forward, reversed, 10);
    }

    [Fact]
    public void Options_RejectOddSimpsonIntervalCount()
    {
        var options = new NumericalAnalysisOptions { IntegrationIntervals = 99 };

        Assert.Throws<ArgumentOutOfRangeException>(options.Validate);
    }
}
