using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerOperationsViewModelTests
{
    [Fact]
    public void AndCommand_AppliesOperandInSelectedRadix()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "F3",
            Operand = "0F",
            InputBase = 16,
            WordSize = 8,
            Signed = false
        };

        viewModel.AndCommand.Execute(null);

        Assert.Equal("3", viewModel.Input);
        Assert.Equal("3", viewModel.Hexadecimal);
        Assert.Equal("AND", viewModel.LastOperation);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void XorCommand_RespectsSelectedWordSize()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "F0",
            Operand = "0F",
            InputBase = 16,
            WordSize = 8,
            Signed = false
        };

        viewModel.XorCommand.Execute(null);

        Assert.Equal("FF", viewModel.Input);
        Assert.Equal("11111111", viewModel.BitPattern);
    }

    [Fact]
    public void NotCommand_UsesFixedWidthMask()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "F0",
            InputBase = 16,
            WordSize = 8,
            Signed = false
        };

        viewModel.NotCommand.Execute(null);

        Assert.Equal("F", viewModel.Input);
        Assert.Equal("00001111", viewModel.BitPattern);
        Assert.Equal("NOT", viewModel.LastOperation);
    }

    [Fact]
    public void ShiftCommands_ApplyConfiguredShiftCount()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "3",
            InputBase = 10,
            WordSize = 8,
            ShiftCount = 2,
            Signed = false
        };

        viewModel.ShiftLeftCommand.Execute(null);
        Assert.Equal("12", viewModel.Input);

        viewModel.LogicalShiftRightCommand.Execute(null);
        Assert.Equal("3", viewModel.Input);
        Assert.Equal("LSHR 2", viewModel.LastOperation);
    }

    [Fact]
    public void ShiftCommand_RejectsCountBeyondWordSize()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "1",
            InputBase = 10,
            WordSize = 8,
            ShiftCount = 9
        };

        viewModel.ShiftLeftCommand.Execute(null);

        Assert.Contains("word size", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
