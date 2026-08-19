using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerBitGridViewModelTests
{
    [Fact]
    public void Bits_MatchSelectedWordSizeAndDescendingIndexes()
    {
        var viewModel = new ProgrammerViewModel
        {
            WordSize = 8,
            Input = "5",
            InputBase = 10
        };

        viewModel.ConvertCommand.Execute(null);

        Assert.Equal(8, viewModel.Bits.Count);
        Assert.Equal(7, viewModel.Bits[0].Index);
        Assert.Equal(0, viewModel.Bits[^1].Index);
        Assert.True(viewModel.Bits.Single(bit => bit.Index == 2).IsSet);
        Assert.True(viewModel.Bits.Single(bit => bit.Index == 0).IsSet);
    }

    [Fact]
    public void BitCellToggleCommand_UpdatesValueAndCellState()
    {
        var viewModel = new ProgrammerViewModel
        {
            WordSize = 8,
            Input = "0",
            InputBase = 10
        };
        viewModel.ConvertCommand.Execute(null);
        var bit3 = viewModel.Bits.Single(bit => bit.Index == 3);

        bit3.ToggleCommand.Execute(null);

        Assert.Equal("8", viewModel.Input);
        Assert.True(bit3.IsSet);
        Assert.Equal("b3: 1", bit3.Label);
        Assert.Equal("Bit 3, set", bit3.AccessibleLabel);
    }

    [Fact]
    public void WordSizeChange_RebuildsBitCollection()
    {
        var viewModel = new ProgrammerViewModel { Input = "0", InputBase = 10 };

        viewModel.WordSize = 128;

        Assert.Equal(128, viewModel.Bits.Count);
        Assert.Equal(127, viewModel.Bits[0].Index);
    }
}
