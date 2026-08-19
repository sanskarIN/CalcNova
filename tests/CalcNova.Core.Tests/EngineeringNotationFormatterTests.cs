using CalcNova.Core.Numerics;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class EngineeringNotationFormatterTests
{
    [Theory]
    [InlineData(0d, "0")]
    [InlineData(1d, "1")]
    [InlineData(12.5d, "12.5")]
    [InlineData(1234d, "1.234e+3")]
    [InlineData(1200000d, "1.2e+6")]
    [InlineData(0.0012d, "1.2e-3")]
    [InlineData(-0.0000045d, "-4.5e-6")]
    public void Format_UsesExponentMultiplesOfThree(double value, string expected)
    {
        Assert.Equal(expected, EngineeringNotationFormatter.Format(value));
    }

    [Fact]
    public void Format_RoundingAcrossThousandBoundary_AdvancesExponent()
    {
        var formatted = EngineeringNotationFormatter.Format(999999.9999999999d, significantDigits: 12);

        Assert.Equal("1e+6", formatted);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(15)]
    public void Format_AcceptsSupportedSignificantDigits(int digits)
    {
        Assert.NotEmpty(EngineeringNotationFormatter.Format(1234.56789d, digits));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    [InlineData(-1)]
    public void Format_RejectsUnsupportedSignificantDigits(int digits)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EngineeringNotationFormatter.Format(1d, digits));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Format_RejectsNonFiniteValues(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EngineeringNotationFormatter.Format(value));
    }

    [Theory]
    [InlineData("1.234e+3", 1234d)]
    [InlineData("12.5", 12.5d)]
    [InlineData("1.2E-3", 0.0012d)]
    [InlineData("-4.5e-6", -0.0000045d)]
    [InlineData("0e+300", 0d)]
    public void Parse_AcceptsCanonicalEngineeringNotation(string text, double expected)
    {
        Assert.Equal(expected, EngineeringNotationFormatter.Parse(text), 12);
    }

    [Theory]
    [InlineData("1e2")]
    [InlineData("1e4")]
    [InlineData("0.5e3")]
    [InlineData("1000e3")]
    [InlineData("1e")]
    [InlineData("e3")]
    [InlineData("1e3e6")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    public void Parse_RejectsInvalidEngineeringNotation(string text)
    {
        Assert.ThrowsAny<Exception>(() => EngineeringNotationFormatter.Parse(text));
    }

    [Theory]
    [InlineData("1e+309")]
    [InlineData("1e-327")]
    [InlineData("0e+309")]
    [InlineData("0e-327")]
    public void Parse_RejectsExponentOutsideFiniteEngineeringRange(string text)
    {
        Assert.Throws<OverflowException>(() => EngineeringNotationFormatter.Parse(text));
    }

    [Fact]
    public void Parse_RejectsUnderflowingNonZeroEngineeringValue()
    {
        Assert.Throws<OverflowException>(() => EngineeringNotationFormatter.Parse("1e-324"));
    }

    [Fact]
    public void Parse_RejectsInputAboveCharacterBudget()
    {
        var text = new string('1', EngineeringNotationFormatter.MaximumInputCharacters + 1);

        Assert.Throws<ArgumentException>(() => EngineeringNotationFormatter.Parse(text));
    }

    [Fact]
    public void Parse_RejectsOversizedWhitespaceBeforeScanningForBlankInput()
    {
        var text = new string(' ', EngineeringNotationFormatter.MaximumInputCharacters + 1);

        Assert.Throws<ArgumentException>(() => EngineeringNotationFormatter.Parse(text));
    }

    [Fact]
    public void FormatAndParse_RoundTripRepresentativeFiniteValues()
    {
        double[] values =
        [
            1d,
            -12.345d,
            0.000000987654321d,
            1234567890123d,
            double.Epsilon,
            double.MaxValue
        ];

        foreach (var value in values)
        {
            var formatted = EngineeringNotationFormatter.Format(value, 15);
            var parsed = EngineeringNotationFormatter.Parse(formatted);
            var relativeError = Math.Abs((parsed - value) / value);
            Assert.True(relativeError <= 1e-14 || parsed.Equals(value), $"Round trip failed for {value}: {formatted} -> {parsed}");
        }
    }
}
