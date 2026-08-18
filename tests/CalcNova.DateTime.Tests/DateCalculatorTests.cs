using CalcNova.DateTimeTools;
using Xunit;

namespace CalcNova.DateTime.Tests;

public sealed class DateCalculatorTests
{
    [Fact]
    public void Difference_ReturnsSignedAndAbsoluteDayCounts()
    {
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 1, 10);

        var forward = DateCalculator.Difference(start, end);
        var reverse = DateCalculator.Difference(end, start);

        Assert.Equal(9, forward.SignedDays);
        Assert.Equal(9, forward.AbsoluteDays);
        Assert.Equal(1, forward.WholeWeeks);
        Assert.Equal(2, forward.RemainingDays);
        Assert.Equal(-9, reverse.SignedDays);
        Assert.Equal(9, reverse.AbsoluteDays);
    }

    [Fact]
    public void Add_MonthAtLeapYearEnd_UsesCalendarSemantics()
    {
        var result = DateCalculator.Add(new DateOnly(2024, 1, 31), months: 1);

        Assert.Equal(new DateOnly(2024, 2, 29), result);
    }

    [Theory]
    [InlineData(2024, true)]
    [InlineData(2100, false)]
    [InlineData(2000, true)]
    public void IsLeapYear_UsesGregorianRules(int year, bool expected)
    {
        Assert.Equal(expected, DateCalculator.IsLeapYear(year));
    }

    [Fact]
    public void BusinessDaysBetween_IsDirectionalAndExcludesWeekends()
    {
        var monday = new DateOnly(2026, 1, 5);
        var nextMonday = new DateOnly(2026, 1, 12);

        Assert.Equal(5, DateCalculator.BusinessDaysBetween(monday, nextMonday));
        Assert.Equal(-5, DateCalculator.BusinessDaysBetween(nextMonday, monday));
    }

    [Fact]
    public void DurationConverter_ConvertsFixedUnitsExactlyForSimpleValues()
    {
        Assert.Equal(90d, DurationConverter.Convert(1.5d, DurationUnit.Hour, DurationUnit.Minute), 12);
        Assert.Equal(168d, DurationConverter.Convert(1d, DurationUnit.Week, DurationUnit.Hour), 12);
        Assert.Equal(0.001d, DurationConverter.Convert(1d, DurationUnit.Millisecond, DurationUnit.Second), 12);
    }

    [Fact]
    public void DurationConverter_RejectsNonFiniteInput()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DurationConverter.Convert(double.PositiveInfinity, DurationUnit.Second, DurationUnit.Minute));
    }
}
