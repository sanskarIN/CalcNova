using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorSelectionEditorTests
{
    [Fact]
    public void FunctionToken_WrapsForwardSelection()
    {
        var edit = CalculatorSelectionEditor.ApplyToken("1+25", 2, 4, "sqrt(", 1_024);

        Assert.Equal("1+sqrt(25)", edit.Expression);
        Assert.Equal(edit.Expression.Length, edit.CaretIndex);
    }

    [Fact]
    public void FunctionToken_WrapsReversedSelection()
    {
        var edit = CalculatorSelectionEditor.ApplyToken("2+3*4", 5, 2, "sin(", 1_024);

        Assert.Equal("2+sin(3*4)", edit.Expression);
        Assert.Equal(edit.Expression.Length, edit.CaretIndex);
    }

    [Fact]
    public void OpenParenthesis_WrapsSelectedExpression()
    {
        var edit = CalculatorSelectionEditor.ApplyToken("1+2*3", 2, 5, "(", 1_024);

        Assert.Equal("1+(2*3)", edit.Expression);
        Assert.Equal(edit.Expression.Length, edit.CaretIndex);
    }

    [Fact]
    public void FunctionToken_WithoutSelection_RemainsOpenForTyping()
    {
        var edit = CalculatorSelectionEditor.ApplyToken("12", 1, 1, "cos(", 1_024);

        Assert.Equal("1cos(2", edit.Expression);
        Assert.Equal(5, edit.CaretIndex);
    }

    [Fact]
    public void OrdinaryToken_ReplacesSelection()
    {
        var edit = CalculatorSelectionEditor.ApplyToken("12345", 1, 4, "+", 1_024);

        Assert.Equal("1+5", edit.Expression);
        Assert.Equal(2, edit.CaretIndex);
    }

    [Fact]
    public void SelectionIndexes_AreClampedBeforeEditing()
    {
        var edit = CalculatorSelectionEditor.ApplyToken("123", -20, 50, "9", 1_024);

        Assert.Equal("9", edit.Expression);
        Assert.Equal(1, edit.CaretIndex);
    }

    [Fact]
    public void WrappedSelection_RespectsFinalExpressionLimit()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CalculatorSelectionEditor.ApplyToken("12345", 1, 4, "sqrt(", 9));
    }

    [Theory]
    [InlineData("sin(")]
    [InlineData("log10(")]
    [InlineData("sqrt(")]
    [InlineData("factorial(")]
    [InlineData("(")]
    public void WrapperDetection_AcceptsFunctionAndParenthesisPrefixes(string token)
    {
        Assert.True(CalculatorSelectionEditor.IsWrapperToken(token));
    }

    [Theory]
    [InlineData("+")]
    [InlineData("pi")]
    [InlineData(")")]
    [InlineData("9")]
    public void WrapperDetection_RejectsOrdinaryTokens(string token)
    {
        Assert.False(CalculatorSelectionEditor.IsWrapperToken(token));
    }
}
