using System.Numerics;
using CalcNova.Core.Numerics;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class RationalNumberTests
{
    [Theory]
    [InlineData(2, 4, 1, 2)]
    [InlineData(-2, 4, -1, 2)]
    [InlineData(2, -4, -1, 2)]
    [InlineData(-2, -4, 1, 2)]
    [InlineData(0, -99, 0, 1)]
    public void Constructor_NormalizesSignAndGreatestCommonDivisor(
        long numerator,
        long denominator,
        long expectedNumerator,
        long expectedDenominator)
    {
        var value = new RationalNumber(numerator, denominator);

        Assert.Equal(new BigInteger(expectedNumerator), value.Numerator);
        Assert.Equal(new BigInteger(expectedDenominator), value.Denominator);
    }

    [Fact]
    public void Constructor_RejectsZeroDenominator()
    {
        Assert.Throws<DivideByZeroException>(() => new RationalNumber(1, 0));
    }

    [Fact]
    public void DefaultValue_IsCanonicalZero()
    {
        RationalNumber value = default;

        Assert.Equal(RationalNumber.Zero, value);
        Assert.Equal(BigInteger.Zero, value.Numerator);
        Assert.Equal(BigInteger.One, value.Denominator);
        Assert.True(value.IsInteger);
        Assert.Equal("0", value.ToString());
        Assert.Equal(RationalNumber.One, value + RationalNumber.One);
        Assert.Equal(0, value.CompareTo(RationalNumber.Zero));
    }

    [Theory]
    [InlineData("42", "42")]
    [InlineData("-7/21", "-1/3")]
    [InlineData("0.125", "1/8")]
    [InlineData("-12.50", "-25/2")]
    [InlineData("1.25e3", "1250")]
    [InlineData("1.25e-3", "1/800")]
    [InlineData(".5", "1/2")]
    [InlineData("5.", "5")]
    public void Parse_ProducesCanonicalExactRepresentation(string text, string expected)
    {
        Assert.Equal(expected, RationalNumber.Parse(text).ToString());
    }

    [Fact]
    public void Parse_DecimalPointOne_IsExactlyOneTenth()
    {
        var value = RationalNumber.Parse("0.1");

        Assert.Equal(BigInteger.One, value.Numerator);
        Assert.Equal(new BigInteger(10), value.Denominator);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1/2/3")]
    [InlineData("1/")]
    [InlineData("/2")]
    [InlineData("1.2.3")]
    [InlineData("1e2e3")]
    [InlineData("1ehello")]
    [InlineData("+")]
    [InlineData(".")]
    public void Parse_RejectsInvalidSyntax(string text)
    {
        Assert.ThrowsAny<Exception>(() => RationalNumber.Parse(text));
    }

    [Fact]
    public void Parse_RejectsZeroFractionDenominator()
    {
        Assert.Throws<DivideByZeroException>(() => RationalNumber.Parse("1/0"));
    }

    [Theory]
    [InlineData("1e10001")]
    [InlineData("1e-10001")]
    [InlineData("0.0000000001e-10000")]
    public void Parse_RejectsDecimalScaleOutsideWorkloadBudget(string text)
    {
        Assert.Throws<OverflowException>(() => RationalNumber.Parse(text));
    }

    [Fact]
    public void Parse_RejectsInputAboveCharacterBudget()
    {
        var text = new string('1', RationalNumber.MaximumInputCharacters + 1);

        Assert.Throws<ArgumentException>(() => RationalNumber.Parse(text));
    }

    [Fact]
    public void Parse_RejectsWhitespacePaddedInputAboveCharacterBudget()
    {
        var padding = new string(' ', RationalNumber.MaximumInputCharacters);
        var text = $"{padding}1{padding}";

        Assert.Throws<ArgumentException>(() => RationalNumber.Parse(text));
    }

    [Fact]
    public void Constructor_RejectsReducedValuesAboveBitBudget()
    {
        var tooLarge = BigInteger.One << RationalNumber.MaximumBitLength;

        Assert.Throws<OverflowException>(() => new RationalNumber(tooLarge, BigInteger.One));
    }

    [Fact]
    public void Addition_UsesExactReducedArithmetic()
    {
        var result = RationalNumber.Parse("1/6") + RationalNumber.Parse("1/3");

        Assert.Equal("1/2", result.ToString());
    }

    [Fact]
    public void Subtraction_UsesExactReducedArithmetic()
    {
        var result = RationalNumber.Parse("7/10") - RationalNumber.Parse("3/5");

        Assert.Equal("1/10", result.ToString());
    }

    [Fact]
    public void Multiplication_CrossCancelsBeforeFinalConstruction()
    {
        var result = RationalNumber.Parse("123456789/35") * RationalNumber.Parse("70/123456789");

        Assert.Equal("2", result.ToString());
    }

    [Fact]
    public void Division_UsesExactReciprocalArithmetic()
    {
        var result = RationalNumber.Parse("3/4") / RationalNumber.Parse("9/10");

        Assert.Equal("5/6", result.ToString());
    }

    [Fact]
    public void Division_RejectsZeroDivisor()
    {
        Assert.Throws<DivideByZeroException>(() => RationalNumber.One / RationalNumber.Zero);
        Assert.Throws<DivideByZeroException>(() => RationalNumber.Zero.Reciprocal());
    }

    [Fact]
    public void Arithmetic_RejectsFinalMagnitudeAboveBitBudget()
    {
        var nearLimit = new RationalNumber(
            (BigInteger.One << (RationalNumber.MaximumBitLength - 1)) - 1,
            BigInteger.One);

        Assert.Throws<OverflowException>(() => _ = nearLimit * nearLimit);
    }

    [Fact]
    public void Comparison_UsesExactCrossProducts()
    {
        var oneThird = RationalNumber.Parse("1/3");
        var decimalThirdApproximation = RationalNumber.Parse("0.3333333333333333");

        Assert.True(oneThird > decimalThirdApproximation);
        Assert.True(RationalNumber.Parse("-2/3") < RationalNumber.Parse("-1/2"));
        Assert.Equal(0, RationalNumber.Parse("2/4").CompareTo(RationalNumber.Parse("1/2")));
    }

    [Fact]
    public void CanonicalValues_HaveStableEqualityAndHashCodes()
    {
        var left = RationalNumber.Parse("2/4");
        var right = RationalNumber.Parse("0.5");

        Assert.Equal(left, right);
        Assert.True(left == right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Theory]
    [InlineData("3/7", true)]
    [InlineData("1/0", false)]
    [InlineData("not-rational", false)]
    public void TryParse_ReturnsDeterministicSuccessState(string text, bool expectedSuccess)
    {
        var success = RationalNumber.TryParse(text, out var value);

        Assert.Equal(expectedSuccess, success);
        if (!success)
        {
            Assert.Equal(RationalNumber.Zero, value);
        }
    }
}
