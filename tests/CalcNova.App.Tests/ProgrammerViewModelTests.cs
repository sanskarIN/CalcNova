using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerViewModelTests
{
    [Fact]
    public void Convert_SupportsCustomBaseTwoThroughThirtySix()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "10",
            InputBase = 10,
            OutputBase = 3,
            WordSize = 8,
            Signed = false
        };

        viewModel.Convert();

        Assert.Equal("101", viewModel.CustomOutput);
        Assert.Equal("00001010", viewModel.BitPattern);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void ToggleBit_LowBitUpdatesInputAndOutputs()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "0",
            InputBase = 10,
            WordSize = 8,
            Signed = false
        };
        viewModel.Convert();
        var leastSignificant = Assert.Single(viewModel.BitCells, cell => cell.BitIndex == 0);

        viewModel.ToggleBitCommand.Execute(leastSignificant);

        Assert.Equal("1", viewModel.Input);
        Assert.Equal("00000001", viewModel.BitPattern);
        Assert.Equal("1", viewModel.InterpretedValue);
    }

    [Fact]
    public void ToggleBit_SignBitUsesTwosComplementInterpretation()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "0",
            InputBase = 10,
            WordSize = 8,
            Signed = true
        };
        viewModel.Convert();
        var signBit = Assert.Single(viewModel.BitCells, cell => cell.BitIndex == 7);

        viewModel.ToggleBitCommand.Execute(signBit);

        Assert.Equal("-128", viewModel.Input);
        Assert.Equal("10000000", viewModel.BitPattern);
        Assert.Equal("-128", viewModel.InterpretedValue);
    }
}
