using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerCustomRadixViewModelTests
{
    [Fact]
    public void SupportedBases_ExposeEveryRadixFromTwoThroughThirtySix()
    {
        var viewModel = new ProgrammerViewModel();

        Assert.Equal(35, viewModel.SupportedBases.Count);
        Assert.Equal(2, viewModel.SupportedBases[0]);
        Assert.Equal(36, viewModel.SupportedBases[^1]);
    }

    [Fact]
    public void ConvertCommand_HandlesCustomBaseThreeInput()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "10201",
            InputBase = 3,
            WordSize = 16,
            Signed = false
        };

        viewModel.ConvertCommand.Execute(null);

        Assert.Equal("100", viewModel.Decimal);
        Assert.Equal("64", viewModel.Hexadecimal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(37)]
    public void InputBase_RejectsUnsupportedRadix(int radix)
    {
        var viewModel = new ProgrammerViewModel();

        Assert.Throws<ArgumentOutOfRangeException>(() => viewModel.InputBase = radix);
    }
}
