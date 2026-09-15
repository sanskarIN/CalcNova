using CalcNova.DateTimeTools;
using Xunit;

namespace CalcNova.DateTime.Tests;

/// <summary>
/// Invariant coverage for the date and duration helpers.
/// </summary>
/// <remarks>
/// The cases in <see cref="DateCalculatorTests"/> pin a handful of known answers around a
/// single week in 2024. These sweep the supported calendar range instead and assert the
/// relationships the results must satisfy, so a boundary mistake outside that week -
/// year 1, year 9999, a leap day, a week that straddles a month or year end - is caught.
/// </remarks>
public sealed class DateCalculatorInvariantTests
{
    [Fact]
    public void Difference_DecomposesTheDayCountConsistently()
    {
        var start = new DateOnly(2023, 11, 3);

        for (var offset = -800; offset <= 800; offset++)
        {
            var end = start.AddDays(offset);
            var difference = DateCalculator.Difference(start, end);

            Assert.Equal(offset, difference.SignedDays);
            Assert.Equal(Math.Abs(offset), difference.AbsoluteDays);
            Assert.Equal(difference.AbsoluteDays, (difference.WholeWeeks * 7) + difference.RemainingDays);
            Assert.InRange(difference.RemainingDays, 0, 6);
            Assert.True(difference.WholeWeeks >= 0);
        }
    }

    [Fact]
    public void Difference_HandlesTheFullSupportedCalendarSpan()
    {
        var minimum = DateOnly.MinValue;
        var maximum = DateOnly.MaxValue;

        var forward = DateCalculator.Difference(minimum, maximum);
        var backward = DateCalculator.Difference(maximum, minimum);

        Assert.Equal(maximum.DayNumber - minimum.DayNumber, forward.SignedDays);
        Assert.Equal(-forward.SignedDays, backward.SignedDays);
        Assert.Equal(forward.AbsoluteDays, backward.AbsoluteDays);
        Assert.Equal(forward.WholeWeeks, backward.WholeWeeks);
        Assert.Equal(forward.RemainingDays, backward.RemainingDays);
    }

    [Fact]
    public void Add_WithNoComponents_ReturnsTheSameDate()
    {
        for (var offset = 0; offset < 400; offset++)
        {
            var date = new DateOnly(2020, 1, 1).AddDays(offset);

            Assert.Equal(date, DateCalculator.Add(date));
        }
    }

    [Fact]
    public void Add_WeeksAndDays_AgreeWithTheEquivalentDayCount()
    {
        var date = new DateOnly(2024, 2, 28);

        for (var weeks = -20; weeks <= 20; weeks++)
        {
            for (var days = -10; days <= 10; days++)
            {
                Assert.Equal(
                    DateCalculator.Add(date, days: (weeks * 7) + days),
                    DateCalculator.Add(date, weeks: weeks, days: days));
            }
        }
    }

    [Fact]
    public void Add_DayOffsets_RoundTripThroughTheirNegation()
    {
        var date = new DateOnly(1999, 12, 31);

        for (var days = -500; days <= 500; days++)
        {
            var moved = DateCalculator.Add(date, days: days);

            Assert.Equal(date, DateCalculator.Add(moved, days: -days));
        }
    }

    [Fact]
    public void Add_YearOffsets_LandOnTheSameCalendarDayExceptAcrossLeapDays()
    {
        // 29 February has no counterpart in a common year; every other day does.
        var leapDay = new DateOnly(2024, 2, 29);
        Assert.Equal(new DateOnly(2025, 2, 28), DateCalculator.Add(leapDay, years: 1));
        Assert.Equal(new DateOnly(2028, 2, 29), DateCalculator.Add(leapDay, years: 4));

        var ordinaryDay = new DateOnly(2024, 3, 1);
        for (var years = -20; years <= 20; years++)
        {
            var moved = DateCalculator.Add(ordinaryDay, years: years);

            Assert.Equal(ordinaryDay.Month, moved.Month);
            Assert.Equal(ordinaryDay.Day, moved.Day);
            Assert.Equal(ordinaryDay.Year + years, moved.Year);
        }
    }

    [Fact]
    public void BusinessDaysBetween_MatchesAnIndependentDayByDayCount()
    {
        var origin = new DateOnly(2024, 1, 1);

        for (var offset = -400; offset <= 400; offset++)
        {
            var start = origin;
            var end = origin.AddDays(offset);

            Assert.Equal(CountBusinessDays(start, end), DateCalculator.BusinessDaysBetween(start, end));
        }
    }

    [Fact]
    public void BusinessDaysBetween_NeverExceedsTheCalendarDayCount()
    {
        var origin = new DateOnly(2023, 6, 15);

        for (var offset = -300; offset <= 300; offset++)
        {
            var end = origin.AddDays(offset);
            var businessDays = DateCalculator.BusinessDaysBetween(origin, end);

            Assert.True(
                Math.Abs(businessDays) <= Math.Abs(offset),
                $"{Math.Abs(businessDays)} business days over {Math.Abs(offset)} calendar days.");

            if (businessDays != 0)
            {
                Assert.Equal(Math.Sign(offset), Math.Sign(businessDays));
            }
        }
    }

    [Fact]
    public void BusinessDaysBetween_CountsFiveForEveryWholeWeek()
    {
        var monday = new DateOnly(2024, 4, 1);
        Assert.Equal(DayOfWeek.Monday, monday.DayOfWeek);

        for (var weeks = 1; weeks <= 52; weeks++)
        {
            Assert.Equal(weeks * 5, DateCalculator.BusinessDaysBetween(monday, monday.AddDays(weeks * 7)));
        }
    }

    [Fact]
    public void BusinessDaysBetween_CountsTheDayItArrivesOnRatherThanTheDayItLeaves()
    {
        // The walk credits each day it steps onto, so the start date is excluded and the
        // end date included. That makes the count directional rather than antisymmetric
        // when exactly one endpoint falls on a weekend; this pins the behavior so it stays
        // a deliberate convention.
        var friday = new DateOnly(2024, 4, 5);
        var saturday = friday.AddDays(1);
        Assert.Equal(DayOfWeek.Friday, friday.DayOfWeek);

        Assert.Equal(0, DateCalculator.BusinessDaysBetween(friday, saturday));
        Assert.Equal(-1, DateCalculator.BusinessDaysBetween(saturday, friday));
        Assert.Equal(0, DateCalculator.BusinessDaysBetween(friday, friday));
    }

    [Fact]
    public void IsLeapYear_AgreesWithTheGregorianRuleAcrossEverySupportedYear()
    {
        for (var year = 1; year <= 9999; year++)
        {
            var expected = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);

            Assert.Equal(expected, DateCalculator.IsLeapYear(year));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10000)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void IsLeapYear_RejectsYearsOutsideTheSupportedRange(int year) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DateCalculator.IsLeapYear(year));

    [Theory]
    [InlineData(DurationUnit.Millisecond)]
    [InlineData(DurationUnit.Second)]
    [InlineData(DurationUnit.Minute)]
    [InlineData(DurationUnit.Hour)]
    [InlineData(DurationUnit.Day)]
    [InlineData(DurationUnit.Week)]
    public void DurationConverter_ConvertingAUnitToItselfIsTheIdentity(DurationUnit unit)
    {
        foreach (var value in new[] { 0d, 1d, -1d, 0.5d, 1234.5678d, -98765.4321d })
        {
            Assert.Equal(value, DurationConverter.Convert(value, unit, unit), 12);
        }
    }

    [Fact]
    public void DurationConverter_RoundTripsThroughEveryUnitPair()
    {
        var units = Enum.GetValues<DurationUnit>();

        foreach (var from in units)
        {
            foreach (var to in units)
            {
                foreach (var value in new[] { 1d, 7d, 0.25d, -3.5d, 1e6d })
                {
                    var converted = DurationConverter.Convert(value, from, to);
                    var restored = DurationConverter.Convert(converted, to, from);

                    Assert.True(
                        Math.Abs(restored - value) <= 1e-9 * Math.Max(1d, Math.Abs(value)),
                        $"{value} {from} -> {to} -> {from} returned {restored}.");
                }
            }
        }
    }

    [Fact]
    public void DurationConverter_ComposesThroughAnIntermediateUnit()
    {
        // A week expressed in seconds must match a week converted via hours.
        var direct = DurationConverter.Convert(3d, DurationUnit.Week, DurationUnit.Second);
        var viaHours = DurationConverter.Convert(
            DurationConverter.Convert(3d, DurationUnit.Week, DurationUnit.Hour),
            DurationUnit.Hour,
            DurationUnit.Second);

        Assert.Equal(direct, viaHours, 6);
        Assert.Equal(1_814_400d, direct, 6);
    }

    [Fact]
    public void DurationConverter_RejectsUndefinedUnits()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DurationConverter.Convert(1d, (DurationUnit)(-1), DurationUnit.Second));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => DurationConverter.Convert(1d, DurationUnit.Second, (DurationUnit)99));
    }

    [Fact]
    public void DurationConverter_ReportsOverflowRatherThanReturningInfinity()
    {
        Assert.Throws<OverflowException>(
            () => DurationConverter.Convert(double.MaxValue, DurationUnit.Week, DurationUnit.Millisecond));
    }

    private static int CountBusinessDays(DateOnly start, DateOnly end)
    {
        if (start == end)
        {
            return 0;
        }

        var step = start < end ? 1 : -1;
        var total = 0;
        for (var cursor = start; cursor != end;)
        {
            cursor = cursor.AddDays(step);
            if (cursor.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            {
                total += step;
            }
        }

        return total;
    }
}
