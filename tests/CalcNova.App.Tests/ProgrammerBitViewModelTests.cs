using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerBitViewModelTests
{
    [Fact]
    public void ToggleBit_UpdatesInputAndAllRepresentations()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "0",
            InputBase = 16,
            WordSize = 8,
            Signed = false
        };

        viewModel.ToggleBit(3);

        Assert.Equal("8", viewModel.Input);
        Assert.Equal("1000", viewModel.Binary);
        Assert.Equal("8", viewModel.Decimal);
        Assert.Equal("8", viewModel.Hexadecimal);
        Assert.EndsWith("1000", viewModel.BitPattern, StringComparison.Ordinal);
        Assert.Equal("8", viewModel.InterpretedValue);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void ToggleBit_InvalidIndexProducesFriendlyError()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "0",
            InputBase = 10,
            WordSize = 8
        };

        viewModel.ToggleBit(8);

        Assert.Contains("Bit index", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
