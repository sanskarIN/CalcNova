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
}
