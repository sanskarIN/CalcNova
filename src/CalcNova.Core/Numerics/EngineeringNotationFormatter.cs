using System.Globalization;

namespace CalcNova.Core.Numerics;

public static class EngineeringNotationFormatter
{
    public const int MaximumInputCharacters = 4_096;
    public const int MinimumSignificantDigits = 1;
    public const int MaximumSignificantDigits = 15;
    public const int MinimumEngineeringExponent = -324;
    public const int MaximumEngineeringExponent = 306;

    public static string Format(double value, int significantDigits = 12)
    {
        ValidateSignificantDigits(significantDigits);
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Engineering notation requires a finite value.");
        }

        if (value == 0d)
        {
            return "0";
        }

        var exponent = GetEngineeringExponent(Math.Abs(value));
        var mantissa = ScaleByPowerOfTen(value, -exponent);
        var roundedMantissa = double.Parse(
            mantissa.ToString($"G{significantDigits}", CultureInfo.InvariantCulture),
            NumberStyles.Float,
            CultureInfo.InvariantCulture);

        if (Math.Abs(roundedMantissa) >= 1000d && exponent <= MaximumEngineeringExponent - 3)
        {
            roundedMantissa /= 1000d;
            exponent += 3;
        }

        var mantissaText = roundedMantissa.ToString($"G{significantDigits}", CultureInfo.InvariantCulture);
        return exponent == 0 ? mantissaText : $"{mantissaText}e{exponent:+0;-0;0}";
    }

    public static double Parse(string? text)
    {
        if (text is null)
        {
            throw new FormatException("Engineering notation text is required.");
        }

        if (text.Length > MaximumInputCharacters)
        {
            throw new ArgumentException(
                $"Engineering notation input may contain at most {MaximumInputCharacters} characters.",
                nameof(text));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new FormatException("Engineering notation text is required.");
        }

        var trimmed = text.Trim();
        var exponentMarker = trimmed.IndexOfAny(['e', 'E']);
        if (exponentMarker < 0)
        {
            return ParseFinite(trimmed);
        }

        if (trimmed.IndexOfAny(['e', 'E'], exponentMarker + 1) >= 0)
        {
            throw new FormatException("Engineering notation may contain only one exponent marker.");
        }

        var mantissaText = trimmed[..exponentMarker];
        var exponentText = trimmed[(exponentMarker + 1)..];
        var mantissa = ParseFinite(mantissaText);
        if (!int.TryParse(exponentText, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var exponent))
        {
            throw new FormatException("Engineering exponent must be an integer.");
        }

        if (exponent % 3 != 0)
        {
            throw new FormatException("Engineering exponent must be a multiple of 3.");
        }

        if (exponent is < MinimumEngineeringExponent or > MaximumEngineeringExponent)
        {
            throw new OverflowException(
                $"Engineering exponent must be from {MinimumEngineeringExponent} through {MaximumEngineeringExponent}.");
        }

        if (mantissa != 0d && Math.Abs(mantissa) is < 1d or >= 1000d)
        {
            throw new FormatException(
                "Engineering mantissa must have an absolute value from 1 (inclusive) to 1000 (exclusive)."
            );
        }

        var value = ScaleByPowerOfTen(mantissa, exponent);
        if (!double.IsFinite(value))
        {
            throw new OverflowException("Engineering notation value exceeds the supported numeric range.");
        }

        if (mantissa != 0d && value == 0d)
        {
            throw new OverflowException("Engineering notation value is below the supported non-zero numeric range.");
        }

        return value;
    }

    private static int GetEngineeringExponent(double absoluteValue)
    {
        var decimalExponent = (int)Math.Floor(Math.Log10(absoluteValue));
        return (int)(Math.Floor(decimalExponent / 3d) * 3d);
    }

    private static double ScaleByPowerOfTen(double value, int exponent)
    {
        if (value == 0d || exponent == 0)
        {
            return value;
        }

        var remaining = exponent;
        var result = value;
        while (remaining != 0)
        {
            var step = Math.Clamp(remaining, -300, 300);
            result *= Math.Pow(10d, step);
            remaining -= step;

            if (!double.IsFinite(result) || result == 0d)
            {
                return result;
            }
        }

        return result;
    }

    private static double ParseFinite(string text)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
        {
            throw new FormatException("Engineering notation must contain a finite invariant-culture number.");
        }

        return value;
    }

    private static void ValidateSignificantDigits(int significantDigits)
    {
        if (significantDigits is < MinimumSignificantDigits or > MaximumSignificantDigits)
        {
            throw new ArgumentOutOfRangeException(
                nameof(significantDigits),
                significantDigits,
                $"Significant digits must be from {MinimumSignificantDigits} through {MaximumSignificantDigits}.");
        }
    }
}
