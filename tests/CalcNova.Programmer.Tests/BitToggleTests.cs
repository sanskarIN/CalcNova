using System.Numerics;
using CalcNova.Programmer;
using Xunit;

namespace CalcNova.Programmer.Tests;

public sealed class BitToggleTests
{
    [Fact]
    public void ToggleBit_SetsAndClearsRequestedBit()
    {
        var value = BigInteger.Zero;

        value = BitwiseCalculator.ToggleBit(value, 3, 8);
        Assert.Equal(new BigInteger(8), value);
        Assert.True(BitwiseCalculator.IsBitSet(value, 3, 8));

        value = BitwiseCalculator.ToggleBit(value, 3, 8);
        Assert.Equal(BigInteger.Zero, value);
        Assert.False(BitwiseCalculator.IsBitSet(value, 3, 8));
    }

    [Fact]
    public void ToggleBit_PreservesWordSizeMask()
    {
        var value = BitwiseCalculator.ToggleBit(new BigInteger(0xFF), 7, 8);

        Assert.Equal(new BigInteger(0x7F), value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    public void BitHelpers_RejectOutOfRangeBitIndex(int bitIndex)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BitwiseCalculator.ToggleBit(BigInteger.Zero, bitIndex, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => BitwiseCalculator.IsBitSet(BigInteger.Zero, bitIndex, 8));
    }
}
