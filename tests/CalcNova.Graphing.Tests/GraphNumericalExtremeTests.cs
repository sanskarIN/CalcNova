using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphNumericalExtremeTests
{
    private readonly GraphNumericalAnalyzer _analyzer = new();

    [Fact]
    public void Derivative_HugeXWithDefaultTinyStep_IsRejected()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _analyzer.Derivative("x", 1e300));

        Assert.Contains("too small", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Derivative_SamplePointOverflow_IsRejected()
    {
        var options = new NumericalAnalysisOptions { DerivativeStep = double.MaxValue };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            _analyzer.Derivative("x", double.MaxValue, options));

        Assert.Contains("sample points", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FindRoot_ExtremeSymmetricBounds_UsesOverflowSafeMidpoint()
    {
        var root = _analyzer.FindRoot("x", -double.MaxValue, double.MaxValue);

        Assert.Equal(0d, root);
    }

    [Fact]
    public void FindRoot_EndpointRoot_ReturnsEndpointImmediately()
    {
        var root = _analyzer.FindRoot("x - 2", 2d, 10d);

        Assert.Equal(2d, root);
    }

    [Fact]
    public void FindRoot_DiscontinuityAtMidpoint_IsRejected()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _analyzer.FindRoot("1 / x", -1d, 1d));
    }

    [Fact]
    public void Integrate_ExtremeSymmetricBounds_ZeroFunctionAvoidsIntermediateOverflow()
    {
        var options = new NumericalAnalysisOptions { IntegrationIntervals = 100 };

        var integral = _analyzer.Integrate("0", -double.MaxValue, double.MaxValue, options);

        Assert.Equal(0d, integral);
    }

    [Fact]
    public void Integrate_ZeroWidthInterval_IsExactlyZero()
    {
        var integral = _analyzer.Integrate("1 / 0", 5d, 5d);

        Assert.Equal(0d, integral);
    }

    [Fact]
    public void Integrate_DiscontinuityAtSamplePoint_IsRejected()
    {
        var options = new NumericalAnalysisOptions { IntegrationIntervals = 100 };

        Assert.Throws<InvalidOperationException>(() =>
            _analyzer.Integrate("1 / x", -1d, 1d, options));
    }
}
