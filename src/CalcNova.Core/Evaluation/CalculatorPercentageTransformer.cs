using CalcNova.Core.Errors;
using CalcNova.Core.Numerics;
using CalcNova.Core.Parsing;

namespace CalcNova.Core.Evaluation;

public sealed class CalculatorPercentageTransformer
{
    private static readonly NumberValue OneHundred = NumberValue.FromInteger(100);
    private readonly ExpressionEvaluator _evaluator;

    public CalculatorPercentageTransformer(ExpressionEvaluator? evaluator = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
    }

    public PercentageTransformation Transform(string expression, EvaluationOptions? options = null)
    {
        options ??= EvaluationOptions.Default;
        var compiled = _evaluator.Compile(expression, options);

        if (compiled.SyntaxTree is BinaryExpression binary && binary.Operator is (
            TokenKind.Plus or TokenKind.Minus or TokenKind.Star or TokenKind.Slash))
        {
            var left = EvaluateSubExpression(binary.Left, options);
            var right = EvaluateSubExpression(binary.Right, options);
            var percentageValue = binary.Operator is TokenKind.Plus or TokenKind.Minus
                ? left.Multiply(right).Divide(OneHundred)
                : right.Divide(OneHundred);

            var symbol = binary.Operator switch
            {
                TokenKind.Plus => "+",
                TokenKind.Minus => "-",
                TokenKind.Star => "*",
                TokenKind.Slash => "/",
                _ => throw new CalculationException(CalculationErrorCode.InvalidArgument, "Unsupported percentage context.")
            };

            return new PercentageTransformation(
                $"{left.ToDisplayString()} {symbol} {percentageValue.ToDisplayString()}",
                percentageValue);
        }

        var value = _evaluator.Evaluate(compiled, options);
        if (!value.Success)
        {
            var errorCode = value.ErrorCode == CalculationErrorCode.None
                ? CalculationErrorCode.InvalidArgument
                : value.ErrorCode;
            throw new CalculationException(errorCode, value.ErrorMessage ?? "Percentage conversion failed.");
        }

        var standalonePercentage = value.Value.Divide(OneHundred);
        return new PercentageTransformation(standalonePercentage.ToDisplayString(), standalonePercentage);
    }

    private NumberValue EvaluateSubExpression(Expression expression, EvaluationOptions options)
    {
        var result = _evaluator.Evaluate(new CompiledExpression("<percentage>", expression), options);
        if (!result.Success)
        {
            var errorCode = result.ErrorCode == CalculationErrorCode.None
                ? CalculationErrorCode.InvalidArgument
                : result.ErrorCode;
            throw new CalculationException(errorCode, result.ErrorMessage ?? "Percentage conversion failed.");
        }

        return result.Value;
    }
}

public sealed record PercentageTransformation(string TransformedExpression, NumberValue PercentageValue);
