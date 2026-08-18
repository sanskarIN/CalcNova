using CalcNova.Core.Evaluation;

namespace CalcNova.Scientific;

public sealed class ScientificCalculator
{
    private readonly ExpressionEvaluator _evaluator;

    public ScientificCalculator(ExpressionEvaluator? evaluator = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
    }

    public EvaluationResult Evaluate(string expression, AngleUnit angleUnit = AngleUnit.Radians) =>
        _evaluator.Evaluate(expression, new EvaluationOptions { AngleUnit = angleUnit });
}
