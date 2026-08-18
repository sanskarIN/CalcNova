namespace CalcNova.Equations;

public sealed record LinearEquationSolution(EquationSolutionKind Kind, double? Value)
{
    public static LinearEquationSolution Unique(double value) => new(EquationSolutionKind.UniqueReal, value);

    public static LinearEquationSolution None { get; } = new(EquationSolutionKind.NoSolution, null);

    public static LinearEquationSolution Infinite { get; } = new(EquationSolutionKind.InfiniteSolutions, null);
}
