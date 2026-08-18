using System.Numerics;

namespace CalcNova.Equations;

public sealed class EquationSolver
{
    public LinearEquationSolution SolveLinear(double coefficient, double constant, double zeroTolerance = 0d)
    {
        ValidateFinite(coefficient, nameof(coefficient));
        ValidateFinite(constant, nameof(constant));
        ValidateTolerance(zeroTolerance);

        if (IsZero(coefficient, zeroTolerance))
        {
            return IsZero(constant, zeroTolerance)
                ? LinearEquationSolution.Infinite
                : LinearEquationSolution.None;
        }

        return LinearEquationSolution.Unique(-constant / coefficient);
    }

    public QuadraticEquationSolution SolveQuadratic(
        double a,
        double b,
        double c,
        double zeroTolerance = 1e-14d)
    {
        ValidateFinite(a, nameof(a));
        ValidateFinite(b, nameof(b));
        ValidateFinite(c, nameof(c));
        ValidateTolerance(zeroTolerance);

        if (IsZero(a, zeroTolerance))
        {
            var linear = SolveLinear(b, c, zeroTolerance);
            return linear.Kind switch
            {
                EquationSolutionKind.UniqueReal => new QuadraticEquationSolution(
                    EquationSolutionKind.UniqueReal,
                    new Complex(linear.Value!.Value, 0d),
                    null),
                EquationSolutionKind.InfiniteSolutions => new QuadraticEquationSolution(
                    EquationSolutionKind.InfiniteSolutions,
                    null,
                    null),
                _ => new QuadraticEquationSolution(EquationSolutionKind.NoSolution, null, null)
            };
        }

        var discriminant = (b * b) - (4d * a * c);
        if (!double.IsFinite(discriminant))
        {
            throw new OverflowException("The quadratic discriminant is outside the supported floating-point range.");
        }

        if (discriminant > zeroTolerance)
        {
            var squareRoot = Math.Sqrt(discriminant);
            var q = -0.5d * (b + Math.CopySign(squareRoot, b));

            double first;
            double second;
            if (q == 0d)
            {
                first = second = -b / (2d * a);
            }
            else
            {
                first = q / a;
                second = c / q;
            }

            return new QuadraticEquationSolution(
                EquationSolutionKind.TwoReal,
                new Complex(first, 0d),
                new Complex(second, 0d));
        }

        if (discriminant < -zeroTolerance)
        {
            var denominator = 2d * a;
            var real = -b / denominator;
            var imaginary = Math.Sqrt(-discriminant) / denominator;
            return new QuadraticEquationSolution(
                EquationSolutionKind.ComplexPair,
                new Complex(real, imaginary),
                new Complex(real, -imaginary));
        }

        var repeated = -b / (2d * a);
        var root = new Complex(repeated, 0d);
        return new QuadraticEquationSolution(EquationSolutionKind.RepeatedReal, root, root);
    }

    public RootFindingResult FindRootBisection(
        Func<double, double> function,
        double minimum,
        double maximum,
        double tolerance = 1e-12d,
        int maximumIterations = 256)
    {
        ArgumentNullException.ThrowIfNull(function);
        ValidateFinite(minimum, nameof(minimum));
        ValidateFinite(maximum, nameof(maximum));

        if (minimum >= maximum)
        {
            throw new ArgumentException("The minimum bound must be less than the maximum bound.");
        }

        if (!double.IsFinite(tolerance) || tolerance <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), tolerance, "Tolerance must be finite and greater than zero.");
        }

        if (maximumIterations is < 1 or > 100_000)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumIterations), maximumIterations, "Maximum iterations must be between 1 and 100000.");
        }

        var left = minimum;
        var right = maximum;
        var leftValue = function(left);
        var rightValue = function(right);

        if (!double.IsFinite(leftValue) || !double.IsFinite(rightValue))
        {
            return RootFindingResult.Failed("The function is not finite at one or both interval boundaries.");
        }

        if (Math.Abs(leftValue) <= tolerance)
        {
            return RootFindingResult.Found(left, 0);
        }

        if (Math.Abs(rightValue) <= tolerance)
        {
            return RootFindingResult.Found(right, 0);
        }

        if (Math.Sign(leftValue) == Math.Sign(rightValue))
        {
            return RootFindingResult.Failed("Bisection requires an interval whose endpoints have opposite signs.");
        }

        for (var iteration = 1; iteration <= maximumIterations; iteration++)
        {
            var midpoint = left + ((right - left) / 2d);
            var midpointValue = function(midpoint);
            if (!double.IsFinite(midpointValue))
            {
                return RootFindingResult.Failed("The function became non-finite inside the search interval.", iteration);
            }

            if (Math.Abs(midpointValue) <= tolerance || (right - left) / 2d <= tolerance)
            {
                return RootFindingResult.Found(midpoint, iteration);
            }

            if (Math.Sign(leftValue) == Math.Sign(midpointValue))
            {
                left = midpoint;
                leftValue = midpointValue;
            }
            else
            {
                right = midpoint;
                rightValue = midpointValue;
            }
        }

        return RootFindingResult.Failed("The bisection solver reached the configured iteration limit.", maximumIterations);
    }

    private static bool IsZero(double value, double tolerance) => Math.Abs(value) <= tolerance;

    private static void ValidateFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Equation coefficients must be finite.");
        }
    }

    private static void ValidateTolerance(double tolerance)
    {
        if (!double.IsFinite(tolerance) || tolerance < 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), tolerance, "Zero tolerance must be finite and non-negative.");
        }
    }
}
