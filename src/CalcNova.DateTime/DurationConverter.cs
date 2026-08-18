namespace CalcNova.DateTimeTools;

public static class DurationConverter
{
    public static double Convert(double value, DurationUnit from, DurationUnit to)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Duration value must be finite.");
        }

        if (!Enum.IsDefined(from))
        {
            throw new ArgumentOutOfRangeException(nameof(from));
        }

        if (!Enum.IsDefined(to))
        {
            throw new ArgumentOutOfRangeException(nameof(to));
        }

        var milliseconds = value * MillisecondsPer(from);
        var converted = milliseconds / MillisecondsPer(to);
        if (!double.IsFinite(converted))
        {
            throw new OverflowException("Converted duration is outside the supported floating-point range.");
        }

        return converted == 0d ? 0d : converted;
    }

    private static double MillisecondsPer(DurationUnit unit) => unit switch
    {
        DurationUnit.Millisecond => 1d,
        DurationUnit.Second => 1_000d,
        DurationUnit.Minute => 60_000d,
        DurationUnit.Hour => 3_600_000d,
        DurationUnit.Day => 86_400_000d,
        DurationUnit.Week => 604_800_000d,
        _ => throw new ArgumentOutOfRangeException(nameof(unit))
    };
}
