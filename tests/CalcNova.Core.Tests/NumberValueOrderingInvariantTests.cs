using System.Globalization;
using System.Numerics;
using CalcNova.Core.Numerics;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class NumberValueOrderingInvariantTests
{
    private static readonly NumberValue[] Values =
    [
        NumberValue.FromInteger(BigInteger.Parse("-9007199254740993", CultureInfo.InvariantCulture)),
        NumberValue.FromDouble(-9007199254740992d),
        NumberValue.FromDecimal(-0.1m),
        NumberValue.FromDouble(-0.1d),
        NumberValue.FromDouble(-0d),
        NumberValue.Zero,
        NumberValue.FromDecimal(0.1m),
        NumberValue.FromDouble(0.1d),
        NumberValue.FromDecimal(0.5m),
        NumberValue.FromDouble(0.5d),
        NumberValue.FromInteger(BigInteger.One),
        NumberValue.FromDecimal(1.0m),
        NumberValue.FromDouble(1d),
        NumberValue.FromInteger(BigInteger.Parse("9007199254740992", CultureInfo.InvariantCulture)),
        NumberValue.FromInteger(BigInteger.Parse("9007199254740993", CultureInfo.InvariantCulture))
    ];

    [Fact]
    public void CompareTo_IsAntisymmetricAndConsistentWithEquality()
    {
        foreach (var left in Values)
        {
            foreach (var right in Values)
            {
                var leftToRight = Math.Sign(left.CompareTo(right));
                var rightToLeft = Math.Sign(right.CompareTo(left));

                Assert.Equal(-leftToRight, rightToLeft);
                Assert.Equal(leftToRight == 0, left.Equals(right));

                if (left.Equals(right))
                {
                    Assert.Equal(left.GetHashCode(), right.GetHashCode());
                }
            }
        }
    }

    [Fact]
    public void CompareTo_IsTransitiveForRepresentativeMixedKinds()
    {
        foreach (var first in Values)
        {
            foreach (var second in Values)
            {
                if (first.CompareTo(second) > 0)
                {
                    continue;
                }

                foreach (var third in Values)
                {
                    if (second.CompareTo(third) <= 0)
                    {
                        Assert.True(first.CompareTo(third) <= 0);
                    }
                }
            }
        }
    }
}
