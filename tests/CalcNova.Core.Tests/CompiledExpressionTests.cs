using System.Numerics;
using CalcNova.Core.Errors;
using CalcNova.Core.Evaluation;
using CalcNova.Core.Numerics;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class CompiledExpressionTests
{
    private readonly ExpressionEvaluator _evaluator = new();

    [Fact]
    public void CompiledExpression_CanBeReusedWithDifferentVariables()
    {
        var compiled = _evaluator.Compile("x ^ 2 + 1");

        var first = _evaluator.Evaluate(compiled, new EvaluationOptions
        {
            Variables = new Dictionary<string, NumberValue>
            {
                ["x"] = NumberValue.FromInteger(new BigInteger(2))
            }
        });
        var second = _evaluator.Evaluate(compiled, new EvaluationOptions
        {
            Variables = new Dictionary<string, NumberValue>
            {
                ["x"] = NumberValue.FromInteger(new BigInteger(3))
            }
        });

        Assert.True(first.Success, first.ErrorMessage);
        Assert.True(second.Success, second.ErrorMessage);
        Assert.Equal("5", first.Value.ToDisplayString());
        Assert.Equal("10", second.Value.ToDisplayString());
    }

    [Fact]
    public void Variables_AreResolvedCaseInsensitively()
    {
        var result = _evaluator.Evaluate("X + 1", new EvaluationOptions
        {
            Variables = new Dictionary<string, NumberValue>
            {
                ["x"] = NumberValue.FromInteger(BigInteger.One)
            }
        });

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal("2", result.Value.ToDisplayString());
    }

    [Fact]
    public void BuiltInConstants_CannotBeShadowedByVariables()
    {
        var result = _evaluator.Evaluate("pi", new EvaluationOptions
        {
            Variables = new Dictionary<string, NumberValue>
            {
                ["pi"] = NumberValue.Zero
            }
        });

        Assert.True(result.Success, result.ErrorMessage);
        Assert.InRange(result.Value.ToDouble(), Math.PI - 1e-15, Math.PI + 1e-15);
    }

    [Fact]
    public void UnknownVariable_ReturnsTypedInvalidArgumentError()
    {
        var result = _evaluator.Evaluate("x + 1");

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.InvalidArgument, result.ErrorCode);
    }
}
