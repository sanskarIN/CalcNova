using CalcNova.Core.Errors;
using CalcNova.Core.Numerics;
using CalcNova.Core.Parsing;

namespace CalcNova.Core.Evaluation;

public sealed class CalculationSession
{
    private readonly ExpressionEvaluator _evaluator;
    private RepeatOperation? _repeatOperation;

    public CalculationSession(ExpressionEvaluator? evaluator = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
    }

    public bool CanRepeat => _repeatOperation is not null;

    public EvaluationResult Evaluate(string expression, EvaluationOptions? options = null)
    {
        options ??= EvaluationOptions.Default;

        CompiledExpression compiled;
        try
        {
            compiled = _evaluator.Compile(expression, options);
        }
        catch (CalculationException exception)
        {
            _repeatOperation = null;
            return EvaluationResult.FromError(exception.Code, exception.Message);
        }
        catch (OverflowException)
        {
            _repeatOperation = null;
            return EvaluationResult.FromError(CalculationErrorCode.NumericOverflow, "The result is outside the supported numeric range.");
        }

        var result = _evaluator.Evaluate(compiled, options);
        if (!result.Success)
        {
            _repeatOperation = null;
            return result;
        }

        _repeatOperation = BuildRepeatOperation(compiled.SyntaxTree, options);
        return result;
    }

    public EvaluationResult Repeat(NumberValue currentValue, EvaluationOptions? options = null)
    {
        options ??= EvaluationOptions.Default;
        if (_repeatOperation is null)
        {
            return EvaluationResult.FromError(CalculationErrorCode.InvalidArgument, "There is no repeatable operation in the current calculator session.");
        }

        try
        {
            var value = _repeatOperation.Operator switch
            {
                TokenKind.Plus => currentValue.Add(_repeatOperation.RightOperand),
                TokenKind.Minus => currentValue.Subtract(_repeatOperation.RightOperand),
                TokenKind.Star => currentValue.Multiply(_repeatOperation.RightOperand),
                TokenKind.Slash => currentValue.Divide(_repeatOperation.RightOperand),
                TokenKind.Percent => currentValue.Modulo(_repeatOperation.RightOperand),
                TokenKind.Caret => currentValue.Power(_repeatOperation.RightOperand, options.MaximumIntegerExponent),
                _ => throw new CalculationException(CalculationErrorCode.InvalidArgument, "The previous operation cannot be repeated.")
            };

            return EvaluationResult.FromValue(value);
        }
        catch (CalculationException exception)
        {
            return EvaluationResult.FromError(exception.Code, exception.Message);
        }
        catch (OverflowException)
        {
            return EvaluationResult.FromError(CalculationErrorCode.NumericOverflow, "The repeated operation exceeded the supported numeric range.");
        }
    }

    public void Reset() => _repeatOperation = null;

    private RepeatOperation? BuildRepeatOperation(Expression syntaxTree, EvaluationOptions options)
    {
        if (syntaxTree is not BinaryExpression binary || binary.Operator is not (
            TokenKind.Plus or
            TokenKind.Minus or
            TokenKind.Star or
            TokenKind.Slash or
            TokenKind.Percent or
            TokenKind.Caret))
        {
            return null;
        }

        var right = _evaluator.Evaluate(new CompiledExpression("<repeat-operand>", binary.Right), options);
        return right.Success ? new RepeatOperation(binary.Operator, right.Value) : null;
    }

    private sealed record RepeatOperation(TokenKind Operator, NumberValue RightOperand);
}
