using System.Numerics;

namespace CalcNova.Programmer;

public static class RadixConverter
{
    private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static BigInteger Parse(string text, int radix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ValidateRadix(radix);

        var span = text.Trim().Replace("_", string.Empty, StringComparison.Ordinal).AsSpan();
        var sign = BigInteger.One;
        if (span[0] is '+' or '-')
        {
            if (span[0] == '-')
            {
                sign = BigInteger.MinusOne;
            }

            span = span[1..];
        }

        if (span.IsEmpty)
        {
            throw new FormatException("A radix value must contain at least one digit.");
        }

        var value = BigInteger.Zero;
        foreach (var character in span)
        {
            var digit = DigitValue(character);
            if (digit < 0 || digit >= radix)
            {
                throw new FormatException($"Digit '{character}' is not valid in base {radix}.");
            }

            value = (value * radix) + digit;
        }

        return value * sign;
    }

    public static string Format(BigInteger value, int radix)
    {
        ValidateRadix(radix);
        if (value.IsZero)
        {
            return "0";
        }

        var negative = value.Sign < 0;
        var remaining = BigInteger.Abs(value);
        var characters = new List<char>();

        while (remaining > 0)
        {
            remaining = BigInteger.DivRem(remaining, radix, out var remainder);
            characters.Add(Digits[(int)remainder]);
        }

        if (negative)
        {
            characters.Add('-');
        }

        characters.Reverse();
        return new string([.. characters]);
    }

    private static int DigitValue(char character)
    {
        var upper = char.ToUpperInvariant(character);
        if (upper is >= '0' and <= '9')
        {
            return upper - '0';
        }

        if (upper is >= 'A' and <= 'Z')
        {
            return upper - 'A' + 10;
        }

        return -1;
    }

    private static void ValidateRadix(int radix)
    {
        if (radix is < 2 or > 36)
        {
            throw new ArgumentOutOfRangeException(nameof(radix), radix, "Radix must be between 2 and 36.");
        }
    }
}
