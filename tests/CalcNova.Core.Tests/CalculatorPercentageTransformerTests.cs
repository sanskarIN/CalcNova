using CalcNova.Core.Evaluation;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class CalculatorPercentageTransformerTests
{
    private readonly CalculatorPercentageTransformer _transformer = new();
    private readonly ExpressionEvaluator _evaluator = new();

    [Theory]
    [InlineData("50 + 10", "55")]
    [InlineData("50 - 10", "45")]
    [InlineData("50 * 10", "5")]
    [InlineData("50 / 10", "500")]
    public void Transform_BinaryCalculatorContext_ProducesExpectedResult(string expression, string expected)
    {
        var transformed = _transformer.Transform(expression);
        var evaluated = _evaluator.Evaluate(transformed.TransformedExpression);

        Assert.True(evaluated.Success, evaluated.ErrorMessage);
        Assert.Equal(expected, evaluated.Value.ToDisplayString());
    }

    [Fact]
    public void Transform_StandaloneValue_ReturnsFractionalPercentage()
    {
        var transformed = _transformer.Transform("25");

        Assert.Equal("0.25", transformed.TransformedExpression);
        Assert.Equal("0.25", transformed.PercentageValue.ToDisplayString());
    }

    [Fact]
    public void Transform_Addition_UsesEvaluatedLeftAndRightExpressions()
    {
        var transformed = _transformer.Transform("(40 + 10) + (5 * 2)");
        var evaluated = _evaluator.Evaluate(transformed.TransformedExpression);

        Assert.True(evaluated.Success, evaluated.ErrorMessage);
        Assert.Equal("55", evaluated.Value.ToDisplayString());
    }
}
