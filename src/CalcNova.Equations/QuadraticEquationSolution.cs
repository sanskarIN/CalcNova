using System.Numerics;

namespace CalcNova.Equations;

public sealed record QuadraticEquationSolution(
    EquationSolutionKind Kind,
    Complex? FirstRoot,
    Complex? SecondRoot)
{
    public bool HasRealRoots => Kind is EquationSolutionKind.TwoReal or EquationSolutionKind.RepeatedReal or EquationSolutionKind.UniqueReal;
}
