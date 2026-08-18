using CalcNova.Equations;
using Xunit;

namespace CalcNova.Equations.Tests;

public sealed class EquationSolverTests
{
    private readonly EquationSolver _solver = new();

    [Fact]
    public void SolveLinear_UniqueRoot_ReturnsValue()
    {
        var solution = _solver.SolveLinear(2d, -8d);

        Assert.Equal(EquationSolutionKind.UniqueReal, solution.Kind);
        Assert.Equal(4d, solution.Value!.Value, 12);
    }

    [Fact]
    public void SolveLinear_DegenerateCases_AreDistinguished()
    {
        Assert.Equal(EquationSolutionKind.InfiniteSolutions, _solver.SolveLinear(0d, 0d).Kind);
        Assert.Equal(EquationSolutionKind.NoSolution, _solver.SolveLinear(0d, 4d).Kind);
    }

    [Fact]
    public void SolveQuadratic_TwoRealRoots_ReturnsBoth()
    {
        var solution = _solver.SolveQuadratic(1d, -5d, 6d);

        Assert.Equal(EquationSolutionKind.TwoReal, solution.Kind);
        var roots = new[] { solution.FirstRoot!.Value.Real, solution.SecondRoot!.Value.Real }.OrderBy(value => value).ToArray();
        Assert.Equal(2d, roots[0], 12);
        Assert.Equal(3d, roots[1], 12);
    }

    [Fact]
    public void SolveQuadratic_RepeatedRoot_IsClassified()
    {
        var solution = _solver.SolveQuadratic(1d, 2d, 1d);

        Assert.Equal(EquationSolutionKind.RepeatedReal, solution.Kind);
        Assert.Equal(-1d, solution.FirstRoot!.Value.Real, 12);
        Assert.Equal(-1d, solution.SecondRoot!.Value.Real, 12);
    }

    [Fact]
    public void SolveQuadratic_ComplexPair_ReturnsConjugates()
    {
        var solution = _solver.SolveQuadratic(1d, 0d, 1d);

        Assert.Equal(EquationSolutionKind.ComplexPair, solution.Kind);
        Assert.Equal(0d, solution.FirstRoot!.Value.Real, 12);
        Assert.Equal(1d, Math.Abs(solution.FirstRoot!.Value.Imaginary), 12);
        Assert.Equal(-solution.FirstRoot.Value.Imaginary, solution.SecondRoot!.Value.Imaginary, 12);
    }

    [Fact]
    public void SolveQuadratic_DegeneratesToLinear()
    {
        var solution = _solver.SolveQuadratic(0d, 2d, -10d);

        Assert.Equal(EquationSolutionKind.UniqueReal, solution.Kind);
        Assert.Equal(5d, solution.FirstRoot!.Value.Real, 12);
        Assert.Null(solution.SecondRoot);
    }

    [Fact]
    public void FindRootBisection_FindsBracketedRoot()
    {
        var result = _solver.FindRootBisection(value => (value * value) - 2d, 1d, 2d);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal(Math.Sqrt(2d), result.Root!.Value, 10);
        Assert.InRange(result.Iterations, 1, 256);
    }

    [Fact]
    public void FindRootBisection_RejectsUnbracketedInterval()
    {
        var result = _solver.FindRootBisection(value => (value * value) + 1d, -1d, 1d);

        Assert.False(result.Success);
        Assert.Null(result.Root);
    }
}
