using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphNumericalEdgeCaseTests
{
    private readonly GraphNumericalAnalyzer _analyzer = new();

    [Fact]
    public void FindRoot_ReturnsLeftEndpointWithinTolerance()
    {
        var root = _analyzer.FindRoot("x", 0d, 1d);

        Assert.Equal(0d, root);
    }

    [Fact]
    public void FindRoot_ReturnsRightEndpointWithinTolerance()
    {
        var root = _analyzer.FindRoot("x", -1d, 0d);

        Assert.Equal(0d, root);
    }

    [Fact]
    public void FindRoot_RejectsNonFiniteBounds()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _analyzer.FindRoot("x", double.NegativeInfinity, 1d));
    }

    [Fact]
    public void FindRoot_StopsAtConfiguredIterationBudget()
    {
        var options = new NumericalAnalysisOptions
        {
            MaximumRootIterations = 1,
            RootTolerance = 1e-15
        };

        Assert.Throws<InvalidOperationException>(() =>
            _analyzer.FindRoot("x ^ 2 - 2", 1d, 2d, options));
    }

    [Fact]
    public void Integrate_EqualBounds_ReturnsZero()
    {
        var integral = _analyzer.Integrate("x ^ 2", 3d, 3d);

        Assert.Equal(0d, integral);
    }

    [Fact]
    public void Integrate_RejectsNonFiniteBound()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _analyzer.Integrate("x", 0d, double.PositiveInfinity));
    }

    [Fact]
    public void Derivative_RejectsNonFiniteX()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _analyzer.Derivative("x", double.NaN));
    }
}
