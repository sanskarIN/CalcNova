using CalcNova.App.ViewModels;
using CalcNova.Converter;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterPrecisionViewModelTests
{
    [Fact]
    public void SignificantDigits_ReformatsConversionResult()
    {
        var viewModel = new ConverterViewModel
        {
            SelectedCategory = UnitCategory.Length,
            Input = "1"
        };
        viewModel.FromUnit = viewModel.AvailableUnits.Single(unit => unit.Id == "m");
        viewModel.ToUnit = viewModel.AvailableUnits.Single(unit => unit.Id == "ft");

        viewModel.SignificantDigits = 6;

        Assert.Equal("3.28084 ft", viewModel.Result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(18)]
    public void SignificantDigits_RejectsUnsupportedPrecision(int precision)
    {
        var viewModel = new ConverterViewModel();

        Assert.Throws<ArgumentOutOfRangeException>(() => viewModel.SignificantDigits = precision);
    }
}
