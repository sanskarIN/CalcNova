using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class RationalNumberViewModelTests
{
    [Fact]
    public void NormalizeCommand_ReducesFractionsAndExactDecimals()
    {
        var viewModel = new RationalNumberViewModel
        {
            LeftText = "6/8",
            RightText = "0.125"
        };

        viewModel.NormalizeCommand.Execute(null);

        Assert.Equal("3/4", viewModel.LeftCanonical);
        Assert.Equal("1/8", viewModel.RightCanonical);
        Assert.Equal("Left = 3/4 • Right = 1/8", viewModel.OperationSummary);
        Assert.Empty(viewModel.Result);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Theory]
    [InlineData("add", "1/3", "1/6", "1/2", "1/3 + 1/6 = 1/2")]
    [InlineData("subtract", "3/4", "1/2", "1/4", "3/4 − 1/2 = 1/4")]
    [InlineData("multiply", "2/3", "9/10", "3/5", "2/3 × 9/10 = 3/5")]
    [InlineData("divide", "3/4", "9/10", "5/6", "3/4 ÷ 9/10 = 5/6")]
    public void ArithmeticCommands_ReturnCanonicalExactResults(
        string operation,
        string left,
        string right,
        string expectedResult,
        string expectedSummary)
    {
        var viewModel = new RationalNumberViewModel
        {
            LeftText = left,
            RightText = right
        };

        var command = operation switch
        {
            "add" => viewModel.AddCommand,
            "subtract" => viewModel.SubtractCommand,
            "multiply" => viewModel.MultiplyCommand,
            "divide" => viewModel.DivideCommand,
            _ => throw new InvalidOperationException()
        };
        command.Execute(null);

        Assert.Equal(expectedResult, viewModel.Result);
        Assert.Equal(expectedSummary, viewModel.OperationSummary);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void ArithmeticCommand_UsesExactDecimalInputRatherThanBinaryApproximation()
    {
        var viewModel = new RationalNumberViewModel
        {
            LeftText = "0.1",
            RightText = "0.2"
        };

        viewModel.AddCommand.Execute(null);

        Assert.Equal("3/10", viewModel.Result);
    }

    [Fact]
    public void DivideCommand_RejectsZeroAndClearsStaleOutput()
    {
        var viewModel = new RationalNumberViewModel
        {
            LeftText = "1/2",
            RightText = "1/4"
        };
        viewModel.DivideCommand.Execute(null);
        Assert.NotEmpty(viewModel.Result);

        viewModel.RightText = "0";
        viewModel.DivideCommand.Execute(null);

        Assert.Empty(viewModel.LeftCanonical);
        Assert.Empty(viewModel.RightCanonical);
        Assert.Empty(viewModel.Result);
        Assert.Empty(viewModel.OperationSummary);
        Assert.Contains("zero", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NormalizeCommand_RejectsInvalidInputAndClearsStaleOutput()
    {
        var viewModel = new RationalNumberViewModel
        {
            LeftText = "1/2",
            RightText = "1/3"
        };
        viewModel.AddCommand.Execute(null);
        Assert.Equal("5/6", viewModel.Result);

        viewModel.LeftText = "not-rational";
        viewModel.NormalizeCommand.Execute(null);

        Assert.Empty(viewModel.Result);
        Assert.Empty(viewModel.OperationSummary);
        Assert.NotEmpty(viewModel.ErrorMessage);
    }
}
