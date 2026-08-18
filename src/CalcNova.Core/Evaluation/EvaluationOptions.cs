using CalcNova.Core.Numerics;

namespace CalcNova.Core.Evaluation;

public sealed record EvaluationOptions
{
    public static EvaluationOptions Default { get; } = new();

    public AngleUnit AngleUnit { get; init; } = AngleUnit.Radians;

    public IReadOnlyDictionary<string, NumberValue>? Variables { get; init; }

    public int MaximumExpressionLength { get; init; } = 4096;

    public int MaximumFactorialInput { get; init; } = 5000;

    public int MaximumIntegerExponent { get; init; } = 10000;
}
