using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorFunctionSelectionViewModelTests
{
    [Fact]
    public void AppendFunction_WrapsSelectedTextAndRequestsCaretAfterClose()
    {
        var viewModel = new CalculatorViewModel { Expression = "1+25" };
        (int Start, int End)? requestedSelection = null;
        viewModel.SelectionRequested += (start, end) => requestedSelection = (start, end);
        viewModel.UpdateSelection(2, 4);

        viewModel.AppendCommand.Execute("sqrt(");

        Assert.Equal("1+sqrt(25)", viewModel.Expression);
        Assert.Equal((viewModel.Expression.Length, viewModel.Expression.Length), requestedSelection);
        Assert.Empty(viewModel.StatusMessage);
    }

    [Fact]
    public void AppendParenthesis_WrapsSelectedSubexpression()
    {
        var viewModel = new CalculatorViewModel { Expression = "1+2*3" };
        viewModel.UpdateSelection(2, 5);

        viewModel.AppendCommand.Execute("(");

        Assert.Equal("1+(2*3)", viewModel.Expression);
    }

    [Fact]
    public void AppendFunction_AtCaretKeepsFunctionOpenForFurtherTyping()
    {
        var viewModel = new CalculatorViewModel { Expression = "12" };
        viewModel.UpdateSelection(1, 1);

        viewModel.AppendCommand.Execute("sin(");

        Assert.Equal("1sin(2", viewModel.Expression);
    }

    [Fact]
    public void AppendOrdinaryToken_StillReplacesSelectedText()
    {
        var viewModel = new CalculatorViewModel { Expression = "12345" };
        viewModel.UpdateSelection(1, 4);

        viewModel.AppendCommand.Execute("+");

        Assert.Equal("1+5", viewModel.Expression);
    }
}
