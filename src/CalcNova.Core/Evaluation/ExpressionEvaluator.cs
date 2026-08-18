using System.Numerics;
using CalcNova.Core.Errors;
using CalcNova.Core.Numerics;
using CalcNova.Core.Parsing;

namespace CalcNova.Core.Evaluation;

public sealed class ExpressionEvaluator
{
    public EvaluationResult Evaluate(string expression, EvaluationOptions? options = null)
    {
        options ??= EvaluationOptions.Default;

        if (string.IsNullOrWhiteSpace(expression))
        {
            return EvaluationResult.FromError(CalculationErrorCode.EmptyExpression, "Enter an expression to calculate.");
        }

        if (expression.Length > options.MaximumExpressionLength)
        {
            return EvaluationResult.FromError(CalculationErrorCode.InputTooLong, "The expression exceeds CalcNova's configured input limit.");
        }

        try
        {
            var tokens = new Tokenizer(expression).Tokenize();
            var syntaxTree = new Parser(tokens).Parse();
            return EvaluationResult.FromValue(EvaluateExpression(syntaxTree, options));
        }
        catch (CalculationException exception)
        {
            return EvaluationResult.FromError(exception.Code, exception.Message);
        }
        catch (OverflowException)
        {
            return EvaluationResult.FromError(CalculationErrorCode.NumericOverflow, "The result is outside the supported numeric range.");
        }
    }

    private NumberValue EvaluateExpression(Expression expression, EvaluationOptions options) => expression switch
    {
        NumberExpression number => NumberValue.Parse(number.Literal),
        ConstantExpression constant => EvaluateConstant(constant.Name),
        UnaryExpression unary => EvaluateUnary(unary, options),
        BinaryExpression binary => EvaluateBinary(binary, options),
        CallExpression call => EvaluateCall(call, options),
        _ => throw new CalculationException(CalculationErrorCode.SyntaxError, "Unsupported expression node.")
    };

    private NumberValue EvaluateUnary(UnaryExpression expression, EvaluationOptions options)
    {
        var value = EvaluateExpression(expression.Operand, options);
        return expression.Operator switch
        {
            TokenKind.Plus => value,
            TokenKind.Minus => value.Negate(),
            _ => throw new CalculationException(CalculationErrorCode.SyntaxError, "Unsupported unary operator.")
        };
    }

    private NumberValue EvaluateBinary(BinaryExpression expression, EvaluationOptions options)
    {
        var left = EvaluateExpression(expression.Left, options);
        var right = EvaluateExpression(expression.Right, options);

        return expression.Operator switch
        {
            TokenKind.Plus => left.Add(right),
            TokenKind.Minus => left.Subtract(right),
            TokenKind.Star => left.Multiply(right),
            TokenKind.Slash => left.Divide(right),
            TokenKind.Percent => left.Modulo(right),
            TokenKind.Caret => left.Power(right, options.MaximumIntegerExponent),
            _ => throw new CalculationException(CalculationErrorCode.SyntaxError, "Unsupported binary operator.")
        };
    }

    private NumberValue EvaluateCall(CallExpression expression, EvaluationOptions options)
    {
        var name = expression.Name.ToLowerInvariant();
        var arguments = expression.Arguments.Select(argument => EvaluateExpression(argument, options)).ToArray();

        return name switch
        {
            "sqrt" => UnaryDouble(name, arguments, value => value >= 0d, Math.Sqrt),
            "cbrt" => UnaryDouble(name, arguments, _ => true, Math.Cbrt),
            "abs" => UnaryExact(name, arguments, value => value.Abs()),
            "sqr" or "square" => Power(arguments, NumberValue.FromInteger(2), options, name),
            "cube" => Power(arguments, NumberValue.FromInteger(3), options, name),
            "pow" => PowerFunction(arguments, options, name),
            "root" or "nroot" => Root(arguments, name),
            "inv" or "reciprocal" => Reciprocal(arguments, name),
            "ln" => UnaryDouble(name, arguments, value => value > 0d, Math.Log),
            "log" => Log(arguments, name),
            "log10" => UnaryDouble(name, arguments, value => value > 0d, Math.Log10),
            "log2" => UnaryDouble(name, arguments, value => value > 0d, Math.Log2),
            "exp" => UnaryDouble(name, arguments, _ => true, Math.Exp),
            "sin" => Trig(arguments, options.AngleUnit, Math.Sin, name),
            "cos" => Trig(arguments, options.AngleUnit, Math.Cos, name),
            "tan" => Trig(arguments, options.AngleUnit, Math.Tan, name),
            "asin" => InverseTrig(arguments, options.AngleUnit, value => value is >= -1d and <= 1d, Math.Asin, name),
            "acos" => InverseTrig(arguments, options.AngleUnit, value => value is >= -1d and <= 1d, Math.Acos, name),
            "atan" => InverseTrig(arguments, options.AngleUnit, _ => true, Math.Atan, name),
            "sinh" => UnaryDouble(name, arguments, _ => true, Math.Sinh),
            "cosh" => UnaryDouble(name, arguments, _ => true, Math.Cosh),
            "tanh" => UnaryDouble(name, arguments, _ => true, Math.Tanh),
            "asinh" => UnaryDouble(name, arguments, _ => true, Math.Asinh),
            "acosh" => UnaryDouble(name, arguments, value => value >= 1d, Math.Acosh),
            "atanh" => UnaryDouble(name, arguments, value => value is > -1d and < 1d, Math.Atanh),
            "floor" => WholeNumberTransform(arguments, Math.Floor, name),
            "ceil" or "ceiling" => WholeNumberTransform(arguments, Math.Ceiling, name),
            "trunc" or "truncate" => WholeNumberTransform(arguments, Math.Truncate, name),
            "round" => Round(arguments, name),
            "sign" => Sign(arguments, name),
            "min" => Minimum(arguments, name),
            "max" => Maximum(arguments, name),
            "factorial" or "fact" => Factorial(arguments, options, name),
            "gcd" => GreatestCommonDivisor(arguments, name),
            "lcm" => LeastCommonMultiple(arguments, name),
            "comb" or "ncr" => Combinations(arguments, options, name),
            "perm" or "npr" => Permutations(arguments, options, name),
            "mod" => Modulo(arguments, name),
            "pct" or "percent" => Percentage(arguments, name),
            _ => throw new CalculationException(CalculationErrorCode.UnsupportedFunction, $"Function '{expression.Name}' is not supported.")
        };
    }

    private static NumberValue EvaluateConstant(string name) => name.ToLowerInvariant() switch
    {
        "pi" or "π" => NumberValue.FromDouble(Math.PI),
        "e" => NumberValue.FromDouble(Math.E),
        "tau" or "τ" => NumberValue.FromDouble(Math.Tau),
        _ => throw new CalculationException(CalculationErrorCode.InvalidArgument, $"Unknown constant '{name}'.")
    };

    private static NumberValue UnaryExact(string name, IReadOnlyList<NumberValue> arguments, Func<NumberValue, NumberValue> operation)
    {
        RequireCount(name, arguments, 1);
        return operation(arguments[0]);
    }

    private static NumberValue UnaryDouble(
        string name,
        IReadOnlyList<NumberValue> arguments,
        Func<double, bool> domain,
        Func<double, double> operation)
    {
        RequireCount(name, arguments, 1);
        var value = arguments[0].ToDouble();
        if (!domain(value))
        {
            throw Domain(name);
        }

        var result = operation(value);
        if (double.IsNaN(result))
        {
            throw Domain(name);
        }

        return NumberValue.FromDouble(result);
    }

    private static NumberValue Power(IReadOnlyList<NumberValue> arguments, NumberValue exponent, EvaluationOptions options, string name)
    {
        RequireCount(name, arguments, 1);
        return arguments[0].Power(exponent, options.MaximumIntegerExponent);
    }

    private static NumberValue PowerFunction(IReadOnlyList<NumberValue> arguments, EvaluationOptions options, string name)
    {
        RequireCount(name, arguments, 2);
        return arguments[0].Power(arguments[1], options.MaximumIntegerExponent);
    }

    private static NumberValue Root(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 2);
        if (!arguments[1].TryGetInteger(out var degree) || degree.IsZero || degree < int.MinValue || degree > int.MaxValue)
        {
            throw new CalculationException(CalculationErrorCode.InvalidArgument, "Root degree must be a non-zero integer in the supported range.");
        }

        var degree32 = (int)degree;
        var value = arguments[0].ToDouble();
        if (value < 0d && Math.Abs((long)degree32) % 2 == 0)
        {
            throw Domain(name);
        }

        var magnitude = Math.Pow(Math.Abs(value), 1d / degree32);
        var result = value < 0d ? -magnitude : magnitude;
        return NumberValue.FromDouble(result);
    }

    private static NumberValue Reciprocal(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 1);
        return NumberValue.One.Divide(arguments[0]);
    }

    private static NumberValue Log(IReadOnlyList<NumberValue> arguments, string name)
    {
        if (arguments.Count == 1)
        {
            return UnaryDouble(name, arguments, value => value > 0d, Math.Log10);
        }

        RequireCount(name, arguments, 2);
        var value = arguments[0].ToDouble();
        var @base = arguments[1].ToDouble();
        if (value <= 0d || @base <= 0d || @base == 1d)
        {
            throw Domain(name);
        }

        return NumberValue.FromDouble(Math.Log(value, @base));
    }

    private static NumberValue Trig(IReadOnlyList<NumberValue> arguments, AngleUnit unit, Func<double, double> operation, string name)
    {
        RequireCount(name, arguments, 1);
        return NumberValue.FromDouble(operation(ToRadians(arguments[0].ToDouble(), unit)));
    }

    private static NumberValue InverseTrig(
        IReadOnlyList<NumberValue> arguments,
        AngleUnit unit,
        Func<double, bool> domain,
        Func<double, double> operation,
        string name)
    {
        RequireCount(name, arguments, 1);
        var value = arguments[0].ToDouble();
        if (!domain(value))
        {
            throw Domain(name);
        }

        return NumberValue.FromDouble(FromRadians(operation(value), unit));
    }

    private static NumberValue WholeNumberTransform(IReadOnlyList<NumberValue> arguments, Func<double, double> operation, string name)
    {
        RequireCount(name, arguments, 1);
        var result = operation(arguments[0].ToDouble());
        return NumberValue.FromInteger(new BigInteger(result));
    }

    private static NumberValue Round(IReadOnlyList<NumberValue> arguments, string name)
    {
        if (arguments.Count == 1)
        {
            return NumberValue.FromDouble(Math.Round(arguments[0].ToDouble(), MidpointRounding.ToEven));
        }

        RequireCount(name, arguments, 2);
        if (!arguments[1].TryGetInteger(out var digits) || digits < 0 || digits > 15)
        {
            throw new CalculationException(CalculationErrorCode.InvalidArgument, "Round precision must be an integer from 0 through 15.");
        }

        return NumberValue.FromDouble(Math.Round(arguments[0].ToDouble(), (int)digits, MidpointRounding.ToEven));
    }

    private static NumberValue Sign(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 1);
        return NumberValue.FromInteger(arguments[0].CompareTo(NumberValue.Zero));
    }

    private static NumberValue Minimum(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireAtLeast(name, arguments, 1);
        var result = arguments[0];
        for (var index = 1; index < arguments.Count; index++)
        {
            if (arguments[index].CompareTo(result) < 0)
            {
                result = arguments[index];
            }
        }

        return result;
    }

    private static NumberValue Maximum(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireAtLeast(name, arguments, 1);
        var result = arguments[0];
        for (var index = 1; index < arguments.Count; index++)
        {
            if (arguments[index].CompareTo(result) > 0)
            {
                result = arguments[index];
            }
        }

        return result;
    }

    private static NumberValue Factorial(IReadOnlyList<NumberValue> arguments, EvaluationOptions options, string name)
    {
        RequireCount(name, arguments, 1);
        var value = RequireNonNegativeInteger(arguments[0], name);
        if (value > options.MaximumFactorialInput)
        {
            throw new CalculationException(CalculationErrorCode.WorkloadLimitExceeded, "Factorial input exceeds CalcNova's configured workload limit.");
        }

        var result = BigInteger.One;
        for (var current = new BigInteger(2); current <= value; current++)
        {
            result *= current;
        }

        return NumberValue.FromInteger(result);
    }

    private static NumberValue GreatestCommonDivisor(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 2);
        var left = RequireInteger(arguments[0], name);
        var right = RequireInteger(arguments[1], name);
        return NumberValue.FromInteger(BigInteger.GreatestCommonDivisor(BigInteger.Abs(left), BigInteger.Abs(right)));
    }

    private static NumberValue LeastCommonMultiple(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 2);
        var left = RequireInteger(arguments[0], name);
        var right = RequireInteger(arguments[1], name);
        if (left.IsZero || right.IsZero)
        {
            return NumberValue.Zero;
        }

        var gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(left), BigInteger.Abs(right));
        return NumberValue.FromInteger(BigInteger.Abs((left / gcd) * right));
    }

    private static NumberValue Combinations(IReadOnlyList<NumberValue> arguments, EvaluationOptions options, string name)
    {
        RequireCount(name, arguments, 2);
        var n = RequireNonNegativeInteger(arguments[0], name);
        var r = RequireNonNegativeInteger(arguments[1], name);
        if (r > n)
        {
            throw Domain(name);
        }

        if (n > options.MaximumFactorialInput)
        {
            throw new CalculationException(CalculationErrorCode.WorkloadLimitExceeded, "Combination input exceeds CalcNova's configured workload limit.");
        }

        r = BigInteger.Min(r, n - r);
        var result = BigInteger.One;
        for (var index = BigInteger.One; index <= r; index++)
        {
            result = result * (n - r + index) / index;
        }

        return NumberValue.FromInteger(result);
    }

    private static NumberValue Permutations(IReadOnlyList<NumberValue> arguments, EvaluationOptions options, string name)
    {
        RequireCount(name, arguments, 2);
        var n = RequireNonNegativeInteger(arguments[0], name);
        var r = RequireNonNegativeInteger(arguments[1], name);
        if (r > n)
        {
            throw Domain(name);
        }

        if (n > options.MaximumFactorialInput)
        {
            throw new CalculationException(CalculationErrorCode.WorkloadLimitExceeded, "Permutation input exceeds CalcNova's configured workload limit.");
        }

        var result = BigInteger.One;
        for (var index = BigInteger.Zero; index < r; index++)
        {
            result *= n - index;
        }

        return NumberValue.FromInteger(result);
    }

    private static NumberValue Modulo(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 2);
        return arguments[0].Modulo(arguments[1]);
    }

    private static NumberValue Percentage(IReadOnlyList<NumberValue> arguments, string name)
    {
        RequireCount(name, arguments, 1);
        return arguments[0].Divide(NumberValue.FromInteger(100));
    }

    private static BigInteger RequireInteger(NumberValue value, string name)
    {
        if (!value.TryGetInteger(out var integer))
        {
            throw new CalculationException(CalculationErrorCode.InvalidArgument, $"Function '{name}' requires integer arguments.");
        }

        return integer;
    }

    private static BigInteger RequireNonNegativeInteger(NumberValue value, string name)
    {
        var integer = RequireInteger(value, name);
        if (integer < 0)
        {
            throw Domain(name);
        }

        return integer;
    }

    private static void RequireCount(string name, IReadOnlyCollection<NumberValue> arguments, int expected)
    {
        if (arguments.Count != expected)
        {
            throw new CalculationException(CalculationErrorCode.InvalidArgument, $"Function '{name}' expects {expected} argument(s), but received {arguments.Count}.");
        }
    }

    private static void RequireAtLeast(string name, IReadOnlyCollection<NumberValue> arguments, int minimum)
    {
        if (arguments.Count < minimum)
        {
            throw new CalculationException(CalculationErrorCode.InvalidArgument, $"Function '{name}' expects at least {minimum} argument(s).");
        }
    }

    private static CalculationException Domain(string name) =>
        new(CalculationErrorCode.DomainError, $"Function '{name}' is undefined for the supplied value(s) in the real-number domain.");

    private static double ToRadians(double value, AngleUnit unit) => unit switch
    {
        AngleUnit.Radians => value,
        AngleUnit.Degrees => value * Math.PI / 180d,
        AngleUnit.Gradians => value * Math.PI / 200d,
        _ => value
    };

    private static double FromRadians(double value, AngleUnit unit) => unit switch
    {
        AngleUnit.Radians => value,
        AngleUnit.Degrees => value * 180d / Math.PI,
        AngleUnit.Gradians => value * 200d / Math.PI,
        _ => value
    };
}
