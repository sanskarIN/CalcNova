using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorSelectionEditingTests
{
    [Fact]
    public void AppendCommand_ReplacesForwardSelectionAndRequestsCaretAfterToken()
    {
        var viewModel = new CalculatorViewModel { Expression = "12345" };
        var requests = new List<(int Start, int End)>();
        viewModel.SelectionRequested += (start, end) => requests.Add((start, end));
        viewModel.UpdateSelection(1, 4);

        viewModel.AppendCommand.Execute("9");

        Assert.Equal("195", viewModel.Expression);
        Assert.Equal((2, 2), Assert.Single(requests));
    }

    [Fact]
    public void AppendCommand_ReplacesReversedSelection()
    {
        var viewModel = new CalculatorViewModel { Expression = "12345" };
        viewModel.UpdateSelection(4, 1);

        viewModel.AppendCommand.Execute("+");

        Assert.Equal("1+5", viewModel.Expression);
    }

    [Fact]
    public void AppendCommand_AtCaret_InsertsInsteadOfAlwaysAppending()
    {
        var viewModel = new CalculatorViewModel { Expression = "1234" };
        viewModel.UpdateSelection(2, 2);

        viewModel.AppendCommand.Execute("9");

        Assert.Equal("12934", viewModel.Expression);
    }

    [Fact]
    public void Backspace_WithSelection_RemovesSelectedText()
    {
        var viewModel = new CalculatorViewModel { Expression = "12345" };
        var requests = new List<(int Start, int End)>();
        viewModel.SelectionRequested += (start, end) => requests.Add((start, end));
        viewModel.UpdateSelection(1, 4);

        viewModel.Backspace();

        Assert.Equal("15", viewModel.Expression);
        Assert.Equal((1, 1), Assert.Single(requests));
    }

    [Fact]
    public void Backspace_AtCaret_RemovesCharacterBeforeCaret()
    {
        var viewModel = new CalculatorViewModel { Expression = "12345" };
        viewModel.UpdateSelection(3, 3);

        viewModel.Backspace();

        Assert.Equal("1245", viewModel.Expression);
    }

    [Fact]
    public void Backspace_AtStart_DoesNothing()
    {
        var viewModel = new CalculatorViewModel { Expression = "123" };
        viewModel.UpdateSelection(0, 0);

        viewModel.Backspace();

        Assert.Equal("123", viewModel.Expression);
    }

    [Fact]
    public void UpdateSelection_ClampsOutOfRangeIndexes()
    {
        var viewModel = new CalculatorViewModel { Expression = "12345" };

        viewModel.UpdateSelection(-20, 50);
        viewModel.AppendCommand.Execute("9");

        Assert.Equal("9", viewModel.Expression);
    }

    [Fact]
    public void Clear_RequestsCaretAtStart()
    {
        var viewModel = new CalculatorViewModel { Expression = "123" };
        (int Start, int End)? request = null;
        viewModel.SelectionRequested += (start, end) => request = (start, end);

        viewModel.Clear();

        Assert.Equal(string.Empty, viewModel.Expression);
        Assert.Equal((0, 0), request);
    }
}
