using System.Numerics;
using Xunit;

namespace CalcNova.Equations.Tests;

/// <summary>
/// Invariant and numerical-stability coverage for <see cref="EquationSolver"/>.
/// </summary>
/// <remarks>
/// The hand-picked cases in <see cref="EquationSolverTests"/> pin known answers for
/// well-behaved coefficients. These assert that whatever the solver returns actually
/// satisfies the equation it was given, across a deterministic spread of coefficients and
/// including the cancellation-prone shapes where a textbook quadratic formula silently
/// loses the smaller root.
/// </remarks>
public sealed class EquationSolverInvariantTests
{
    private readonly EquationSolver _solver = new();

    [Fact]
    public void SolveQuadratic_KeepsTheSmallRootUnderCatastrophicCancellation()
    {
        // x^2 + 1e8*x + 1: the roots are about -1e8 and -1e-8. Evaluating
        // (-b + sqrt(b^2 - 4ac)) / 2a directly cancels to zero here and loses the small
        // root entirely, so this pins the stable formulation.
        var solution = _solver.SolveQuadratic(1d, 1e8d, 1d);

        Assert.Equal(EquationSolutionKind.TwoReal, solution.Kind);

        var roots = new[] { solution.FirstRoot!.Value.Real, solution.SecondRoot!.Value.Real };
        var small = roots.MinBy(root => Math.Abs(root));
        var large = roots.MaxBy(root => Math.Abs(root));

        // Compare relatively: the two roots differ by sixteen orders of magnitude, so no
        // single decimal-place budget is meaningful for both.
        Assert.True(Math.Abs((small / -1e-8d) - 1d) <= 1e-9d, $"Small root was {small}, expected about -1e-8.");
        Assert.True(Math.Abs((large / -1e8d) - 1d) <= 1e-9d, $"Large root was {large}, expected about -1e8.");
        Assert.NotEqual(0d, small);
    }

    [Theory]
    [InlineData(1d, 1e10d, 1d)]
    [InlineData(1d, -1e10d, 1d)]
    [InlineData(1e-6d, 1e6d, 1e-6d)]
    [InlineData(3d, 1e9d, 7d)]
    public void SolveQuadratic_BothRootsSatisfyTheEquation_EvenWhenWidelySeparated(double a, double b, double c)
    {
        var solution = _solver.SolveQuadratic(a, b, c);

        Assert.Equal(EquationSolutionKind.TwoReal, solution.Kind);
        AssertSatisfiesEquation(a, b, c, solution.FirstRoot!.Value);
        AssertSatisfiesEquation(a, b, c, solution.SecondRoot!.Value);
    }

    [Fact]
    public void SolveQuadratic_RootsSatisfyTheEquation_AcrossGeneratedCoefficients()
    {
        var random = new DeterministicValues(seed: 20260915u);

        for (var trial = 0; trial < 400; trial++)
        {
            var a = random.NextSigned();
            var b = random.NextSigned();
            var c = random.NextSigned();

            var solution = _solver.SolveQuadratic(a, b, c);

            switch (solution.Kind)
            {
                case EquationSolutionKind.TwoReal:
                case EquationSolutionKind.ComplexPair:
                    AssertSatisfiesEquation(a, b, c, solution.FirstRoot!.Value);
                    AssertSatisfiesEquation(a, b, c, solution.SecondRoot!.Value);
                    break;
                case EquationSolutionKind.RepeatedReal:
                    Assert.Equal(solution.FirstRoot, solution.SecondRoot);
                    AssertSatisfiesEquation(a, b, c, solution.FirstRoot!.Value);
                    break;
                case EquationSolutionKind.UniqueReal:
                    AssertSatisfiesEquation(a, b, c, solution.FirstRoot!.Value);
                    break;
                default:
                    Assert.Fail($"Unexpected kind {solution.Kind} for a={a}, b={b}, c={c}.");
                    break;
            }
        }
    }

    [Fact]
    public void SolveQuadratic_ComplexRootsAreConjugatesOfEachOther()
    {
        var random = new DeterministicValues(seed: 8080u);

        for (var trial = 0; trial < 200; trial++)
        {
            var a = random.NextSigned();
            var b = random.NextSigned();
            // Force a negative discriminant: b^2 - 4ac < 0 requires ac > b^2/4 with a and c
            // sharing a sign.
            var c = Math.CopySign((b * b / (4d * Math.Abs(a))) + 1d, a);

            var solution = _solver.SolveQuadratic(a, b, c);

            Assert.Equal(EquationSolutionKind.ComplexPair, solution.Kind);

            var first = solution.FirstRoot!.Value;
            var second = solution.SecondRoot!.Value;
            Assert.Equal(first.Real, second.Real, 12);
            Assert.Equal(first.Imaginary, -second.Imaginary, 12);
            Assert.NotEqual(0d, first.Imaginary);
        }
    }

    [Fact]
    public void SolveLinear_ReturnsTheValueThatZeroesTheExpression()
    {
        var random = new DeterministicValues(seed: 606u);

        for (var trial = 0; trial < 200; trial++)
        {
            var coefficient = random.NextSigned();
            var constant = random.NextSigned();

            var solution = _solver.SolveLinear(coefficient, constant);

            Assert.Equal(EquationSolutionKind.UniqueReal, solution.Kind);

            var residual = (coefficient * solution.Value!.Value) + constant;
            Assert.True(
                Math.Abs(residual) <= 1e-12 * Math.Max(1d, Math.Abs(constant)),
                $"{coefficient}x + {constant} = 0 produced residual {residual}.");
        }
    }

    [Fact]
    public void FindRootBisection_ReturnsARootOfTheFunctionItWasGiven()
    {
        var random = new DeterministicValues(seed: 4242u);

        for (var trial = 0; trial < 100; trial++)
        {
            // (x - root) * (x^2 + 3) has exactly one real zero, at `root`, so any bracket
            // containing it has endpoints of opposite sign.
            var root = random.NextSigned();
            double Function(double x) => (x - root) * ((x * x) + 3d);

            var result = _solver.FindRootBisection(Function, -8d, 8d, tolerance: 1e-10d);

            Assert.True(result.Success, result.ErrorMessage);
            Assert.Equal(root, result.Root!.Value, 8);
            Assert.True(Math.Abs(Function(result.Root.Value)) <= 1e-9, "The reported root does not zero the function.");
        }
    }

    [Fact]
    public void FindRootBisection_ReportsAnEndpointThatIsAlreadyARoot()
    {
        var result = _solver.FindRootBisection(x => x, 0d, 5d);

        Assert.True(result.Success);
        Assert.Equal(0d, result.Root!.Value, 12);
        Assert.Equal(0, result.Iterations);
    }

    private static void AssertSatisfiesEquation(double a, double b, double c, Complex root)
    {
        var residual = (a * root * root) + (b * root) + c;

        // Root residuals scale with the coefficients and with the root itself, so the budget
        // has to scale too; a fixed absolute epsilon would reject correct answers for large
        // coefficients and accept wrong ones for tiny coefficients.
        var scale = Math.Max(
            1d,
            (Math.Abs(a) * Complex.Abs(root) * Complex.Abs(root)) + (Math.Abs(b) * Complex.Abs(root)) + Math.Abs(c));

        Assert.True(
            Complex.Abs(residual) <= 1e-9 * scale,
            $"{a}x^2 + {b}x + {c} at {root} left residual {residual} (scale {scale}).");
    }

    /// <summary>A fixed linear congruential generator, so every trial reproduces exactly.</summary>
    private sealed class DeterministicValues(uint seed)
    {
        private uint _state = seed == 0u ? 1u : seed;

        /// <summary>Returns a value in [-5, 5) that is never close enough to zero to be degenerate.</summary>
        public double NextSigned()
        {
            _state = (1664525u * _state) + 1013904223u;
            var unit = ((_state >> 8) & 0xFFFFFF) / (double)0x1000000;
            var value = (unit * 10d) - 5d;
            return Math.Abs(value) < 0.25d ? value + 0.5d : value;
        }
    }
}
