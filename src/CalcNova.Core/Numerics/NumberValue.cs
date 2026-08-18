using System.Globalization;
using System.Numerics;
using CalcNova.Core.Errors;

namespace CalcNova.Core.Numerics;

public enum NumberKind
{
    Integer = 0,
    Decimal,
    Floating
}

public readonly struct NumberValue : IEquatable<NumberValue>, IComparable<NumberValue>
{
    private readonly BigInteger _integer;
    private readonly decimal _decimal;
    private readonly double _floating;

    private NumberValue(BigInteger value)
    {
        Kind = NumberKind.Integer;
        _integer = value;
        _decimal = default;
        _floating = default;
    }

    private NumberValue(decimal value)
    {
        Kind = NumberKind.Decimal;
        _integer = default;
        _decimal = value;
        _floating = default;
    }

    private NumberValue(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new CalculationException(CalculationErrorCode.NumericOverflow, "The result is outside the supported numeric range.");
        }

        Kind = NumberKind.Floating;
        _integer = default;
        _decimal = default;
        _floating = value;
    }

    public NumberKind Kind { get; }

    public static NumberValue Zero { get; } = FromInteger(BigInteger.Zero);

    public static NumberValue One { get; } = FromInteger(BigInteger.One);

    public bool IsZero => Kind switch
    {
        NumberKind.Integer => _integer.IsZero,
        NumberKind.Decimal => _decimal == 0m,
        _ => _floating == 0d
    };

    public static NumberValue FromInteger(BigInteger value) => new(value);

    public static NumberValue FromDecimal(decimal value) => new(value);

    public static NumberValue FromDouble(double value) => new(value);

    public static NumberValue Parse(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        if (!text.Contains('.') && !text.Contains('e') && !text.Contains('E') &&
            BigInteger.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer))
        {
            return FromInteger(integer);
        }

        if (decimal.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var decimalValue))
        {
            return FromDecimal(decimalValue);
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatingValue) &&
            double.IsFinite(floatingValue))
        {
            return FromDouble(floatingValue);
        }

        throw new CalculationException(CalculationErrorCode.NumericOverflow, $"The number '{text}' is outside the supported range.");
    }

    public bool TryGetInteger(out BigInteger value)
    {
        switch (Kind)
        {
            case NumberKind.Integer:
                value = _integer;
                return true;
            case NumberKind.Decimal when decimal.Truncate(_decimal) == _decimal:
                value = new BigInteger(_decimal);
                return true;
            case NumberKind.Floating when double.IsFinite(_floating) && Math.Truncate(_floating) == _floating:
                value = new BigInteger(_floating);
                return true;
            default:
                value = default;
                return false;
        }
    }

    public double ToDouble() => Kind switch
    {
        NumberKind.Integer => (double)_integer,
        NumberKind.Decimal => (double)_decimal,
        _ => _floating
    };

    public NumberValue Add(NumberValue other)
    {
        if (Kind == NumberKind.Integer && other.Kind == NumberKind.Integer)
        {
            return FromInteger(_integer + other._integer);
        }

        if (TryToDecimal(out var left) && other.TryToDecimal(out var right))
        {
            try
            {
                return FromDecimal(checked(left + right));
            }
            catch (OverflowException)
            {
            }
        }

        return FromDouble(ToFiniteDouble(ToDouble() + other.ToDouble()));
    }

    public NumberValue Subtract(NumberValue other) => Add(other.Negate());

    public NumberValue Multiply(NumberValue other)
    {
        if (Kind == NumberKind.Integer && other.Kind == NumberKind.Integer)
        {
            return FromInteger(_integer * other._integer);
        }

        if (TryToDecimal(out var left) && other.TryToDecimal(out var right))
        {
            try
            {
                return FromDecimal(checked(left * right));
            }
            catch (OverflowException)
            {
            }
        }

        return FromDouble(ToFiniteDouble(ToDouble() * other.ToDouble()));
    }

    public NumberValue Divide(NumberValue other)
    {
        if (other.IsZero)
        {
            throw new CalculationException(CalculationErrorCode.DivideByZero, "Division by zero is undefined.");
        }

        if (Kind == NumberKind.Integer && other.Kind == NumberKind.Integer)
        {
            var quotient = BigInteger.DivRem(_integer, other._integer, out var remainder);
            if (remainder.IsZero)
            {
                return FromInteger(quotient);
            }
        }

        if (TryToDecimal(out var left) && other.TryToDecimal(out var right))
        {
            try
            {
                return FromDecimal(left / right);
            }
            catch (OverflowException)
            {
            }
        }

        return FromDouble(ToFiniteDouble(ToDouble() / other.ToDouble()));
    }

    public NumberValue Modulo(NumberValue other)
    {
        if (other.IsZero)
        {
            throw new CalculationException(CalculationErrorCode.DivideByZero, "Modulo by zero is undefined.");
        }

        if (Kind == NumberKind.Integer && other.Kind == NumberKind.Integer)
        {
            return FromInteger(_integer % other._integer);
        }

        if (TryToDecimal(out var left) && other.TryToDecimal(out var right))
        {
            return FromDecimal(left % right);
        }

        return FromDouble(ToFiniteDouble(ToDouble() % other.ToDouble()));
    }

    public NumberValue Power(NumberValue exponent, int maximumIntegerExponent)
    {
        if (exponent.TryGetInteger(out var integerExponent) &&
            integerExponent >= int.MinValue && integerExponent <= int.MaxValue)
        {
            var exponent32 = (int)integerExponent;
            if (Math.Abs((long)exponent32) > maximumIntegerExponent)
            {
                throw new CalculationException(CalculationErrorCode.WorkloadLimitExceeded, "The exponent exceeds CalcNova's configured workload limit.");
            }

            if (exponent32 >= 0 && Kind == NumberKind.Integer)
            {
                return FromInteger(BigInteger.Pow(_integer, exponent32));
            }

            if (exponent32 >= 0 && TryToDecimal(out var decimalBase))
            {
                try
                {
                    return FromDecimal(PowDecimal(decimalBase, exponent32));
                }
                catch (OverflowException)
                {
                }
            }
        }

        var result = Math.Pow(ToDouble(), exponent.ToDouble());
        if (double.IsNaN(result))
        {
            throw new CalculationException(CalculationErrorCode.DomainError, "The power operation is outside the real-number domain.");
        }

        return FromDouble(ToFiniteDouble(result));
    }

    public NumberValue Negate() => Kind switch
    {
        NumberKind.Integer => FromInteger(BigInteger.Negate(_integer)),
        NumberKind.Decimal => FromDecimal(-_decimal),
        _ => FromDouble(-_floating)
    };

    public NumberValue Abs() => Kind switch
    {
        NumberKind.Integer => FromInteger(BigInteger.Abs(_integer)),
        NumberKind.Decimal => FromDecimal(decimal.Abs(_decimal)),
        _ => FromDouble(Math.Abs(_floating))
    };

    public int CompareTo(NumberValue other)
    {
        if (Kind == NumberKind.Integer && other.Kind == NumberKind.Integer)
        {
            return _integer.CompareTo(other._integer);
        }

        if (TryToDecimal(out var left) && other.TryToDecimal(out var right))
        {
            return left.CompareTo(right);
        }

        return ToDouble().CompareTo(other.ToDouble());
    }

    public string ToDisplayString() => Kind switch
    {
        NumberKind.Integer => _integer.ToString(CultureInfo.InvariantCulture),
        NumberKind.Decimal => NormalizeNegativeZero(_decimal).ToString("G29", CultureInfo.InvariantCulture),
        _ => NormalizeNegativeZero(_floating).ToString("G17", CultureInfo.InvariantCulture)
    };

    public override string ToString() => ToDisplayString();

    public bool Equals(NumberValue other) => CompareTo(other) == 0;

    public override bool Equals(object? obj) => obj is NumberValue other && Equals(other);

    public override int GetHashCode() => NormalizeNegativeZero(ToDouble()).GetHashCode();

    private bool TryToDecimal(out decimal value)
    {
        switch (Kind)
        {
            case NumberKind.Integer when _integer >= (BigInteger)decimal.MinValue && _integer <= (BigInteger)decimal.MaxValue:
                value = (decimal)_integer;
                return true;
            case NumberKind.Decimal:
                value = _decimal;
                return true;
            default:
                value = default;
                return false;
        }
    }

    private static decimal PowDecimal(decimal value, int exponent)
    {
        var result = 1m;
        var factor = value;
        var remaining = exponent;

        while (remaining > 0)
        {
            if ((remaining & 1) == 1)
            {
                result = checked(result * factor);
            }

            remaining >>= 1;
            if (remaining > 0)
            {
                factor = checked(factor * factor);
            }
        }

        return result;
    }

    private static double ToFiniteDouble(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new CalculationException(CalculationErrorCode.NumericOverflow, "The result is outside the supported numeric range.");
        }

        return value;
    }

    private static decimal NormalizeNegativeZero(decimal value) => value == 0m ? 0m : value;

    private static double NormalizeNegativeZero(double value) => value == 0d ? 0d : value;
}
