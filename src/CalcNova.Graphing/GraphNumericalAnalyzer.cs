using CalcNova.Core.Evaluation;
using CalcNova.Core.Numerics;

namespace CalcNova.Graphing;

public sealed class GraphNumericalAnalyzer
{
    private readonly ExpressionEvaluator _evaluator;

    public GraphNumericalAnalyzer(ExpressionEvaluator? evaluator = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
    }

    public double Derivative(
        string expression,
        double x,
        NumericalAnalysisOptions? options = null,
        AngleUnit angleUnit = AngleUnit.Radians)
    {
        options ??= new NumericalAnalysisOptions();
        options.Validate();
        ValidateFinite(x, nameof(x));

        var compiled = _evaluator.Compile(expression);
        var step = options.DerivativeStep;
        var leftX = x - step;
        var rightX = x + step;
        if (!double.IsFinite(leftX) || !double.IsFinite(rightX))
        {
            throw new InvalidOperationException("Derivative sample points are outside the supported numeric range.");
        }

        if (leftX == x || rightX == x || leftX == rightX)
        {
            throw new InvalidOperationException("Derivative step is too small relative to the requested x value.");
        }

        var left = EvaluateAt(compiled, leftX, angleUnit);
        var right = EvaluateAt(compiled, rightX, angleUnit);
        return RequireFinite((right - left) / (2d * step), "Derivative result is outside the supported numeric range.");
    }

    public double FindRoot(
        string expression,
        double minimumX,
        double maximumX,
        NumericalAnalysisOptions? options = null,
        AngleUnit angleUnit = AngleUnit.Radians)
    {
        options ??= new NumericalAnalysisOptions();
        options.Validate();
        ValidateInterval(minimumX, maximumX);

        var compiled = _evaluator.Compile(expression);
        var left = minimumX;
        var right = maximumX;
        var leftValue = EvaluateAt(compiled, left, angleUnit);
        var rightValue = EvaluateAt(compiled, right, angleUnit);

        if (Math.Abs(leftValue) <= options.RootTolerance)
        {
            return left;
        }

        if (Math.Abs(rightValue) <= options.RootTolerance)
        {
            return right;
        }

        if (Math.Sign(leftValue) == Math.Sign(rightValue))
        {
            throw new InvalidOperationException("Root interval must bracket a sign change.");
        }

        for (var iteration = 0; iteration < options.MaximumRootIterations; iteration++)
        {
            var middle = SafeMidpoint(left, right);
            if (middle == left || middle == right)
            {
                return middle;
            }

            var middleValue = EvaluateAt(compiled, middle, angleUnit);
            if (Math.Abs(middleValue) <= options.RootTolerance ||
                IntervalWithinTolerance(left, right, options.RootTolerance))
            {
                return middle;
            }

            if (Math.Sign(leftValue) == Math.Sign(middleValue))
            {
                left = middle;
                leftValue = middleValue;
            }
            else
            {
                right = middle;
            }
        }

        throw new InvalidOperationException("Root search did not converge within the configured iteration limit.");
    }

    public double Integrate(
        string expression,
        double minimumX,
        double maximumX,
        NumericalAnalysisOptions? options = null,
        AngleUnit angleUnit = AngleUnit.Radians)
    {
        options ??= new NumericalAnalysisOptions();
        options.Validate();
        ValidateFinite(minimumX, nameof(minimumX));
        ValidateFinite(maximumX, nameof(maximumX));

        if (minimumX == maximumX)
        {
            return 0d;
        }

        if (minimumX > maximumX)
        {
            return -Integrate(expression, maximumX, minimumX, options, angleUnit);
        }

        var compiled = _evaluator.Compile(expression);
        var intervals = options.IntegrationIntervals;
        var width = (maximumX / intervals) - (minimumX / intervals);
        if (!double.IsFinite(width) || width <= 0d)
        {
            throw new InvalidOperationException("Integration interval width is outside the supported numeric range.");
        }

        var sum = EvaluateAt(compiled, minimumX, angleUnit) + EvaluateAt(compiled, maximumX, angleUnit);

        for (var index = 1; index < intervals; index++)
        {
            var x = minimumX + (index * width);
            if (!double.IsFinite(x))
            {
                throw new InvalidOperationException("Integration sample point is outside the supported numeric range.");
            }

            var weight = (index & 1) == 0 ? 2d : 4d;
            sum += weight * EvaluateAt(compiled, x, angleUnit);
        }

        return RequireFinite((width / 3d) * sum, "Integral result is outside the supported numeric range.");
    }

    private double EvaluateAt(CompiledExpression expression, double x, AngleUnit angleUnit)
    {
        var variables = new Dictionary<string, NumberValue>(StringComparer.OrdinalIgnoreCase)
        {
            ["x"] = NumberValue.FromDouble(x)
        };
        var evaluation = _evaluator.Evaluate(expression, new EvaluationOptions
        {
            AngleUnit = angleUnit,
            Variables = variables
        });

        if (!evaluation.Success)
        {
            throw new InvalidOperationException(evaluation.ErrorMessage ?? "Graph expression could not be evaluated.");
        }

        return RequireFinite(evaluation.Value.ToDouble(), "Graph expression produced a non-finite value.");
    }

    private static double SafeMidpoint(double left, double right)
    {
        var midpoint = (left / 2d) + (right / 2d);
        return RequireFinite(midpoint, "Root midpoint is outside the supported numeric range.");
    }

    private static bool IntervalWithinTolerance(double left, double right, double tolerance)
    {
        var halfWidth = Math.Abs((right / 2d) - (left / 2d));
        return halfWidth <= tolerance / 2d;
    }

    private static void ValidateInterval(double minimumX, double maximumX)
    {
        ValidateFinite(minimumX, nameof(minimumX));
        ValidateFinite(maximumX, nameof(maximumX));
        if (minimumX >= maximumX)
        {
            throw new ArgumentException("Root interval minimum must be less than maximum.");
        }
    }

    private static void ValidateFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }
    }

    private static double RequireFinite(double value, string message)
    {
        if (!double.IsFinite(value))
        {
            throw new InvalidOperationException(message);
        }

        return value;
    }
}
