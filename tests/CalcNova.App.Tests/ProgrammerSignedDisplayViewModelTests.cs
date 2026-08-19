using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerSignedDisplayViewModelTests
{
    [Fact]
    public void Convert_SignedEightBitValueKeepsUnsignedNonDecimalRepresentations()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "FF",
            InputBase = 16,
            WordSize = 8,
            Signed = true
        };

        viewModel.ConvertCommand.Execute(null);

        Assert.Equal("11111111", viewModel.Binary);
        Assert.Equal("377", viewModel.Octal);
        Assert.Equal("-1", viewModel.Decimal);
        Assert.Equal("FF", viewModel.Hexadecimal);
        Assert.Equal("-1", viewModel.InterpretedValue);
    }

    [Fact]
    public void ArithmeticShiftRight_PreservesTwosComplementBitPattern()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "80",
            InputBase = 16,
            WordSize = 8,
            Signed = true,
            ShiftCount = 1
        };

        viewModel.ArithmeticShiftRightCommand.Execute(null);

        Assert.Equal("C0", viewModel.Input);
        Assert.Equal("11000000", viewModel.Binary);
        Assert.Equal("-64", viewModel.Decimal);
        Assert.Equal("C0", viewModel.Hexadecimal);
        Assert.Equal("-64", viewModel.InterpretedValue);
    }

    [Fact]
    public void SignedDecimalResultUsesSignedInputWhileOtherRadicesRemainMasked()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "128",
            InputBase = 10,
            WordSize = 8,
            Signed = true
        };

        viewModel.NotCommand.Execute(null);

        Assert.Equal("127", viewModel.Input);
        Assert.Equal("7F", viewModel.Hexadecimal);
        Assert.Equal("127", viewModel.Decimal);
    }
}
