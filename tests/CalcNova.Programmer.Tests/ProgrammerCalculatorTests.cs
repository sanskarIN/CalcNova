using System.Numerics;
using Xunit;

namespace CalcNova.Programmer.Tests;

public sealed class ProgrammerCalculatorTests
{
    [Theory]
    [InlineData("101010", 2, "42")]
    [InlineData("52", 8, "42")]
    [InlineData("42", 10, "42")]
    [InlineData("2A", 16, "42")]
    [InlineData("16", 36, "42")]
    public void RadixParse_ConvertsToExpectedDecimal(string input, int radix, string expected)
    {
        var value = RadixConverter.Parse(input, radix);

        Assert.Equal(BigInteger.Parse(expected), value);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(16)]
    [InlineData(36)]
    public void RadixConversion_RoundTripsLargeInteger(int radix)
    {
        var value = BigInteger.Parse("123456789012345678901234567890");

        var encoded = RadixConverter.Format(value, radix);
        var decoded = RadixConverter.Parse(encoded, radix);

        Assert.Equal(value, decoded);
    }

    [Fact]
    public void ToSigned_InterpretsTwosComplement()
    {
        Assert.Equal(new BigInteger(-1), BitwiseCalculator.ToSigned(255, 8));
        Assert.Equal(new BigInteger(-128), BitwiseCalculator.ToSigned(128, 8));
        Assert.Equal(new BigInteger(127), BitwiseCalculator.ToSigned(127, 8));
    }

    [Fact]
    public void Not_RespectsWordSize()
    {
        Assert.Equal(new BigInteger(240), BitwiseCalculator.Not(15, 8));
    }

    [Fact]
    public void ShiftLeft_TruncatesToWordSize()
    {
        Assert.Equal(BigInteger.Zero, BitwiseCalculator.ShiftLeft(128, 1, 8));
    }

    [Fact]
    public void BitString_UsesConfiguredWidth()
    {
        Assert.Equal("11111111", BitwiseCalculator.ToBitString(-1, 8));
    }
}
