using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class EngineeringNotationViewModelTests
{
    [Fact]
    public void FormatCommand_FormatsFiniteInputWithSelectedPrecision()
    {
        var viewModel = new EngineeringNotationViewModel
        {
            InputText = "1234567",
            SignificantDigits = 6
        };

        viewModel.FormatCommand.Execute(null);

        Assert.Equal("1.23457e+6", viewModel.FormattedText);
        Assert.Equal("1234567", viewModel.ParsedValue);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void ParseCommand_ParsesEngineeringTextAndCanonicalizesDisplay()
    {
        var viewModel = new EngineeringNotationViewModel
        {
            InputText = "12.5e+3",
            SignificantDigits = 8
        };

        viewModel.ParseCommand.Execute(null);

        Assert.Equal("12500", viewModel.ParsedValue);
        Assert.Equal("12.5e+3", viewModel.FormattedText);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void ParseCommand_RejectsNonEngineeringExponentAndClearsStaleOutputs()
    {
        var viewModel = new EngineeringNotationViewModel
        {
            InputText = "1.25e+3"
        };
        viewModel.ParseCommand.Execute(null);
        Assert.NotEmpty(viewModel.FormattedText);

        viewModel.InputText = "1e+4";
        viewModel.ParseCommand.Execute(null);

        Assert.Empty(viewModel.FormattedText);
        Assert.Empty(viewModel.ParsedValue);
        Assert.Contains("multiple of 3", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("-Infinity")]
    [InlineData("not-a-number")]
    public void FormatCommand_RejectsInvalidOrNonFiniteInput(string text)
    {
        var viewModel = new EngineeringNotationViewModel { InputText = text };

        viewModel.FormatCommand.Execute(null);

        Assert.Empty(viewModel.FormattedText);
        Assert.Empty(viewModel.ParsedValue);
        Assert.NotEmpty(viewModel.ErrorMessage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(16)]
    public void FormatCommand_ReportsUnsupportedPrecision(int significantDigits)
    {
        var viewModel = new EngineeringNotationViewModel
        {
            InputText = "1234",
            SignificantDigits = significantDigits
        };

        viewModel.FormatCommand.Execute(null);

        Assert.Empty(viewModel.FormattedText);
        Assert.NotEmpty(viewModel.ErrorMessage);
    }
}
