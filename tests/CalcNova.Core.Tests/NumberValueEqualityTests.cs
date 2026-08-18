using System.Globalization;
using System.Numerics;
using CalcNova.Core.Numerics;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class NumberValueEqualityTests
{
    [Fact]
    public void EqualValuesAcrossKinds_HaveEqualHashCodes()
    {
        var integer = NumberValue.FromInteger(BigInteger.One);
        var decimalValue = NumberValue.FromDecimal(1m);
        var floating = NumberValue.FromDouble(1d);

        Assert.Equal(integer, decimalValue);
        Assert.Equal(decimalValue, floating);
        Assert.Equal(integer, floating);
        Assert.Equal(integer.GetHashCode(), decimalValue.GetHashCode());
        Assert.Equal(decimalValue.GetHashCode(), floating.GetHashCode());
    }

    [Fact]
    public void NegativeZeroAndZero_HaveEqualHashCodes()
    {
        var zero = NumberValue.Zero;
        var negativeZero = NumberValue.FromDouble(-0d);

        Assert.Equal(zero, negativeZero);
        Assert.Equal(zero.GetHashCode(), negativeZero.GetHashCode());
    }

    [Fact]
    public void ExactBinaryFraction_IsEqualAcrossAllKinds()
    {
        var decimalValue = NumberValue.FromDecimal(0.5m);
        var floating = NumberValue.FromDouble(0.5d);

        Assert.Equal(decimalValue, floating);
        Assert.Equal(decimalValue.GetHashCode(), floating.GetHashCode());
    }

    [Fact]
    public void BinaryFloatingApproximation_DoesNotEqualExactDecimalTenth()
    {
        var decimalValue = NumberValue.FromDecimal(0.1m);
        var floating = NumberValue.FromDouble(0.1d);

        Assert.NotEqual(decimalValue, floating);
        Assert.True(decimalValue.CompareTo(floating) < 0);
    }

    [Fact]
    public void AdjacentLargeIntegers_DoNotCollapseThroughDoubleRounding()
    {
        var exactlyRepresentable = NumberValue.FromInteger(BigInteger.Parse("9007199254740992", CultureInfo.InvariantCulture));
        var adjacentInteger = NumberValue.FromInteger(BigInteger.Parse("9007199254740993", CultureInfo.InvariantCulture));
        var floating = NumberValue.FromDouble(9007199254740992d);

        Assert.Equal(exactlyRepresentable, floating);
        Assert.NotEqual(adjacentInteger, floating);
        Assert.NotEqual(exactlyRepresentable, adjacentInteger);
        Assert.True(adjacentInteger.CompareTo(floating) > 0);
    }

    [Fact]
    public void Equality_RemainsTransitiveAcrossInternalKinds()
    {
        var integer = NumberValue.FromInteger(new BigInteger(42));
        var decimalValue = NumberValue.FromDecimal(42.0m);
        var floating = NumberValue.FromDouble(42d);

        Assert.Equal(integer, decimalValue);
        Assert.Equal(decimalValue, floating);
        Assert.Equal(integer, floating);
    }
}
