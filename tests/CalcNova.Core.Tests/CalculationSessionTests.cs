using CalcNova.Core.Evaluation;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class CalculationSessionTests
{
    [Fact]
    public void Repeat_Addition_ReusesRightOperand()
    {
        var session = new CalculationSession();
        var first = session.Evaluate("2 + 3");

        var second = session.Repeat(first.Value);
        var third = session.Repeat(second.Value);

        Assert.True(first.Success, first.ErrorMessage);
        Assert.True(second.Success, second.ErrorMessage);
        Assert.True(third.Success, third.ErrorMessage);
        Assert.Equal("5", first.Value.ToDisplayString());
        Assert.Equal("8", second.Value.ToDisplayString());
        Assert.Equal("11", third.Value.ToDisplayString());
    }

    [Fact]
    public void Repeat_Multiplication_ReusesEvaluatedRightExpression()
    {
        var session = new CalculationSession();
        var first = session.Evaluate("2 * (1 + 2)");

        var second = session.Repeat(first.Value);

        Assert.True(first.Success, first.ErrorMessage);
        Assert.True(second.Success, second.ErrorMessage);
        Assert.Equal("6", first.Value.ToDisplayString());
        Assert.Equal("18", second.Value.ToDisplayString());
    }

    [Fact]
    public void Repeat_NonBinaryExpression_IsUnavailable()
    {
        var session = new CalculationSession();
        var first = session.Evaluate("sqrt(81)");
        var repeated = session.Repeat(first.Value);

        Assert.True(first.Success, first.ErrorMessage);
        Assert.False(repeated.Success);
    }

    [Fact]
    public void Reset_ClearsRepeatOperation()
    {
        var session = new CalculationSession();
        var first = session.Evaluate("10 - 2");
        session.Reset();

        var repeated = session.Repeat(first.Value);

        Assert.False(repeated.Success);
    }
}
