namespace CalcNova.DateTimeTools;

public static class DateCalculator
{
    public static DateDifferenceResult Difference(DateOnly start, DateOnly end)
    {
        var signedDays = end.DayNumber - start.DayNumber;
        var absoluteDays = Math.Abs(signedDays);
        return new DateDifferenceResult(
            signedDays,
            absoluteDays,
            absoluteDays / 7,
            absoluteDays % 7);
    }

    public static DateOnly Add(
        DateOnly date,
        int years = 0,
        int months = 0,
        int weeks = 0,
        int days = 0)
    {
        var result = date;
        if (years != 0)
        {
            result = result.AddYears(years);
        }

        if (months != 0)
        {
            result = result.AddMonths(months);
        }

        var totalDays = checked((weeks * 7) + days);
        if (totalDays != 0)
        {
            result = result.AddDays(totalDays);
        }

        return result;
    }

    public static int BusinessDaysBetween(DateOnly start, DateOnly end)
    {
        if (start == end)
        {
            return 0;
        }

        var direction = start < end ? 1 : -1;
        var cursor = start;
        var count = 0;

        while (cursor != end)
        {
            cursor = cursor.AddDays(direction);
            if (cursor.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            {
                count += direction;
            }
        }

        return count;
    }

    public static bool IsLeapYear(int year)
    {
        if (year is < 1 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be between 1 and 9999.");
        }

        return DateTime.IsLeapYear(year);
    }
}
