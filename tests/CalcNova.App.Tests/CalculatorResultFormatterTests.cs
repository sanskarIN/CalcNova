using System.Globalization;
using CalcNova.App.Services;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorResultFormatterTests
{
    [Fact]
    public void Format_GroupsLargeIntegersWithoutChangingDigits()
    {
        var result = CalculatorResultFormatter.Format(
            "12345678901234567890",
            15,
            useGroupingSeparators: true,
            CultureInfo.GetCultureInfo("en-US"));

        Assert.Equal("12,345,678,901,234,567,890", result);
    }

    [Fact]
    public void Format_UsesIndianGroupingWhenCultureRequiresIt()
    {
        var result = CalculatorResultFormatter.Format(
            "12345678",
            15,
            useGroupingSeparators: true,
            CultureInfo.GetCultureInfo("en-IN"));

        Assert.Equal("1,23,45,678", result);
    }

    [Fact]
    public void Format_AppliesSignificantDigitLimitToDecimal()
    {
        var result = CalculatorResultFormatter.Format(
            "1.234567890123456789",
            8,
            useGroupingSeparators: false,
            CultureInfo.InvariantCulture);

        Assert.Equal("1.2345679", result);
    }

    [Fact]
    public void Format_PreservesCanonicalErrorMarker()
    {
        Assert.Equal("Error", CalculatorResultFormatter.Format("Error", 15, true, CultureInfo.InvariantCulture));
    }
}
