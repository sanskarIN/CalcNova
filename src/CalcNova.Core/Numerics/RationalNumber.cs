using System.Globalization;
using System.Numerics;

namespace CalcNova.Core.Numerics;

public readonly struct RationalNumber : IEquatable<RationalNumber>, IComparable<RationalNumber>
{
    public const int MaximumInputCharacters = 4_096;
    public const int MaximumDecimalScale = 10_000;
    public const int MaximumBitLength = 65_536;

    private readonly BigInteger _numerator;
    private readonly BigInteger _denominator;

    public RationalNumber(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero)
        {
            throw new DivideByZeroException("A rational denominator cannot be zero.");
        }

        if (numerator.IsZero)
        {
            _numerator = BigInteger.Zero;
            _denominator = BigInteger.One;
            return;
        }

        if (denominator.Sign < 0)
        {
            numerator = BigInteger.Negate(numerator);
            denominator = BigInteger.Negate(denominator);
        }

        var divisor = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
        numerator /= divisor;
        denominator /= divisor;
        ValidateMagnitude(numerator, denominator);

        _numerator = numerator;
        _denominator = denominator;
    }

    public BigInteger Numerator => _numerator;

    public BigInteger Denominator => _denominator.IsZero ? BigInteger.One : _denominator;

    public bool IsInteger => Denominator.IsOne;

    public static RationalNumber Zero { get; } = new(BigInteger.Zero, BigInteger.One);

    public static RationalNumber One { get; } = new(BigInteger.One, BigInteger.One);

    public static RationalNumber Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new FormatException("Rational number text is required.");
        }

        if (text.Length > MaximumInputCharacters)
        {
            throw new ArgumentException(
                $"Rational input may contain at most {MaximumInputCharacters} characters.",
                nameof(text));
        }

        var trimmed = text.Trim();
        var slashIndex = trimmed.IndexOf('/');
        if (slashIndex >= 0)
        {
            if (trimmed.IndexOf('/', slashIndex + 1) >= 0)
            {
                throw new FormatException("A rational fraction may contain only one '/'.");
            }

            var numerator = ParseInteger(trimmed[..slashIndex], "numerator");
            var denominator = ParseInteger(trimmed[(slashIndex + 1)..], "denominator");
            return new RationalNumber(numerator, denominator);
        }

        return ParseDecimal(trimmed);
    }

    public static bool TryParse(string? text, out RationalNumber value)
    {
        try
        {
            value = Parse(text);
            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException or DivideByZeroException)
        {
            value = Zero;
            return false;
        }
    }

    public RationalNumber Reciprocal()
    {
        if (Numerator.IsZero)
        {
            throw new DivideByZeroException("Zero does not have a reciprocal.");
        }

        return new RationalNumber(Denominator, Numerator);
    }

    public static RationalNumber operator +(RationalNumber left, RationalNumber right)
    {
        var denominatorGcd = BigInteger.GreatestCommonDivisor(left.Denominator, right.Denominator);
        var leftScale = right.Denominator / denominatorGcd;
        var rightScale = left.Denominator / denominatorGcd;
        var numerator = (left.Numerator * leftScale) + (right.Numerator * rightScale);
        var denominator = left.Denominator * leftScale;
        return new RationalNumber(numerator, denominator);
    }

    public static RationalNumber operator -(RationalNumber left, RationalNumber right) => left + (-right);

    public static RationalNumber operator -(RationalNumber value) =>
        new(BigInteger.Negate(value.Numerator), value.Denominator);

    public static RationalNumber operator *(RationalNumber left, RationalNumber right)
    {
        var leftNumerator = left.Numerator;
        var leftDenominator = left.Denominator;
        var rightNumerator = right.Numerator;
        var rightDenominator = right.Denominator;

        var leftCancellation = BigInteger.GreatestCommonDivisor(BigInteger.Abs(leftNumerator), rightDenominator);
        var rightCancellation = BigInteger.GreatestCommonDivisor(BigInteger.Abs(rightNumerator), leftDenominator);

        leftNumerator /= leftCancellation;
        rightDenominator /= leftCancellation;
        rightNumerator /= rightCancellation;
        leftDenominator /= rightCancellation;

        return new RationalNumber(leftNumerator * rightNumerator, leftDenominator * rightDenominator);
    }

    public static RationalNumber operator /(RationalNumber left, RationalNumber right)
    {
        if (right.Numerator.IsZero)
        {
            throw new DivideByZeroException("Cannot divide by a zero rational value.");
        }

        return left * right.Reciprocal();
    }

    public int CompareTo(RationalNumber other)
    {
        if (Equals(other))
        {
            return 0;
        }

        var left = Numerator * other.Denominator;
        var right = other.Numerator * Denominator;
        return left.CompareTo(right);
    }

    public bool Equals(RationalNumber other) =>
        Numerator.Equals(other.Numerator) && Denominator.Equals(other.Denominator);

    public override bool Equals(object? obj) => obj is RationalNumber other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

    public override string ToString() => IsInteger
        ? Numerator.ToString(CultureInfo.InvariantCulture)
        : string.Create(
            CultureInfo.InvariantCulture,
            $"{Numerator}/{Denominator}");

    public static bool operator ==(RationalNumber left, RationalNumber right) => left.Equals(right);

    public static bool operator !=(RationalNumber left, RationalNumber right) => !left.Equals(right);

    public static bool operator <(RationalNumber left, RationalNumber right) => left.CompareTo(right) < 0;

    public static bool operator <=(RationalNumber left, RationalNumber right) => left.CompareTo(right) <= 0;

    public static bool operator >(RationalNumber left, RationalNumber right) => left.CompareTo(right) > 0;

    public static bool operator >=(RationalNumber left, RationalNumber right) => left.CompareTo(right) >= 0;

    private static RationalNumber ParseDecimal(string text)
    {
        var exponent = 0;
        var exponentMarker = FindExponentMarker(text);
        var mantissaText = text;
        if (exponentMarker >= 0)
        {
            mantissaText = text[..exponentMarker];
            var exponentText = text[(exponentMarker + 1)..];
            if (!int.TryParse(exponentText, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out exponent))
            {
                throw new FormatException("Rational decimal exponent must be an integer.");
            }

            if (Math.Abs((long)exponent) > MaximumDecimalScale)
            {
                throw new OverflowException(
                    $"Rational decimal exponent magnitude may not exceed {MaximumDecimalScale}.");
            }
        }

        if (string.IsNullOrEmpty(mantissaText))
        {
            throw new FormatException("Rational decimal mantissa is required.");
        }

        var sign = 1;
        var unsignedMantissa = mantissaText;
        if (unsignedMantissa[0] is '+' or '-')
        {
            sign = unsignedMantissa[0] == '-' ? -1 : 1;
            unsignedMantissa = unsignedMantissa[1..];
        }

        if (unsignedMantissa.Length == 0)
        {
            throw new FormatException("Rational decimal mantissa must contain digits.");
        }

        var decimalPoint = unsignedMantissa.IndexOf('.');
        if (decimalPoint >= 0 && unsignedMantissa.IndexOf('.', decimalPoint + 1) >= 0)
        {
            throw new FormatException("Rational decimal mantissa may contain only one decimal point.");
        }

        var fractionalDigits = decimalPoint < 0 ? 0 : unsignedMantissa.Length - decimalPoint - 1;
        var digits = decimalPoint < 0
            ? unsignedMantissa
            : unsignedMantissa.Remove(decimalPoint, 1);

        if (digits.Length == 0 || digits.Any(character => character is < '0' or > '9'))
        {
            throw new FormatException("Rational decimal mantissa must contain only decimal digits.");
        }

        var numerator = BigInteger.Parse(digits, NumberStyles.None, CultureInfo.InvariantCulture);
        if (sign < 0)
        {
            numerator = BigInteger.Negate(numerator);
        }

        var scale = (long)fractionalDigits - exponent;
        if (Math.Abs(scale) > MaximumDecimalScale)
        {
            throw new OverflowException(
                $"Rational decimal scale magnitude may not exceed {MaximumDecimalScale}.");
        }

        if (scale >= 0)
        {
            return new RationalNumber(numerator, Pow10((int)scale));
        }

        return new RationalNumber(numerator * Pow10((int)-scale), BigInteger.One);
    }

    private static int FindExponentMarker(string text)
    {
        var lower = text.IndexOf('e');
        var upper = text.IndexOf('E');
        var marker = lower < 0 ? upper : upper < 0 ? lower : Math.Min(lower, upper);
        if (marker < 0)
        {
            return -1;
        }

        var remaining = text[(marker + 1)..];
        if (remaining.IndexOf('e') >= 0 || remaining.IndexOf('E') >= 0)
        {
            throw new FormatException("Rational decimal text may contain only one exponent marker.");
        }

        return marker;
    }

    private static BigInteger ParseInteger(string text, string label)
    {
        if (string.IsNullOrWhiteSpace(text) ||
            !BigInteger.TryParse(text.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value))
        {
            throw new FormatException($"Rational {label} must be an integer.");
        }

        return value;
    }

    private static BigInteger Pow10(int exponent)
    {
        if (exponent is < 0 or > MaximumDecimalScale)
        {
            throw new ArgumentOutOfRangeException(nameof(exponent));
        }

        return BigInteger.Pow(10, exponent);
    }

    private static void ValidateMagnitude(BigInteger numerator, BigInteger denominator)
    {
        if (numerator.GetBitLength() > MaximumBitLength || denominator.GetBitLength() > MaximumBitLength)
        {
            throw new OverflowException(
                $"Exact rational numerator and denominator are limited to {MaximumBitLength} bits after reduction.");
        }
    }
}
