using CalcNova.Core.Errors;
using CalcNova.Core.Evaluation;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class ExpressionEvaluatorTests
{
    private readonly ExpressionEvaluator _evaluator = new();

    [Theory]
    [InlineData("1 + 1", "2")]
    [InlineData("2 + 3 * 4", "14")]
    [InlineData("(2 + 3) * 4", "20")]
    [InlineData("2 ^ 3 ^ 2", "512")]
    [InlineData("-2 ^ 2", "-4")]
    [InlineData("2 ^ -2", "0.25")]
    [InlineData("0.1 + 0.2", "0.3")]
    [InlineData("999999999999999999 + 1", "1000000000000000000")]
    [InlineData("10 % 4", "2")]
    [InlineData("2 × (3 + 4)", "14")]
    [InlineData("1e3 + 24", "1024")]
    public void Evaluate_ReturnsExpectedExactDisplay(string expression, string expected)
    {
        var result = _evaluator.Evaluate(expression);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal(expected, result.Value.ToDisplayString());
    }

    [Fact]
    public void Evaluate_DivideByZero_ReturnsTypedError()
    {
        var result = _evaluator.Evaluate("5 / 0");

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.DivideByZero, result.ErrorCode);
    }

    [Fact]
    public void Evaluate_SquareRootOfNegative_ReturnsDomainError()
    {
        var result = _evaluator.Evaluate("sqrt(-1)");

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.DomainError, result.ErrorCode);
    }

    [Fact]
    public void Evaluate_SineInDegrees_UsesConfiguredAngleUnit()
    {
        var result = _evaluator.Evaluate("sin(30)", new EvaluationOptions { AngleUnit = AngleUnit.Degrees });

        Assert.True(result.Success, result.ErrorMessage);
        Assert.InRange(result.Value.ToDouble(), 0.4999999999999999d, 0.5000000000000001d);
    }

    [Fact]
    public void Evaluate_Factorial_ReturnsArbitraryPrecisionInteger()
    {
        var result = _evaluator.Evaluate("factorial(20)");

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal("2432902008176640000", result.Value.ToDisplayString());
    }

    [Theory]
    [InlineData("gcd(84, 30)", "6")]
    [InlineData("lcm(21, 6)", "42")]
    [InlineData("comb(10, 3)", "120")]
    [InlineData("perm(10, 3)", "720")]
    public void Evaluate_IntegerScientificFunctions_ReturnExpectedValue(string expression, string expected)
    {
        var result = _evaluator.Evaluate(expression);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.Equal(expected, result.Value.ToDisplayString());
    }

    [Fact]
    public void Evaluate_ExpressionOverConfiguredLimit_IsRejected()
    {
        var result = _evaluator.Evaluate("1+1", new EvaluationOptions { MaximumExpressionLength = 2 });

        Assert.False(result.Success);
        Assert.Equal(CalculationErrorCode.InputTooLong, result.ErrorCode);
    }
}
