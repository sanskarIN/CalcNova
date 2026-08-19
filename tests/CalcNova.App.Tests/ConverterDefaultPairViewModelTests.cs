using CalcNova.App.ViewModels;
using CalcNova.Converter;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterDefaultPairViewModelTests
{
    [Fact]
    public void InitialCategory_UsesLengthDefaults()
    {
        var viewModel = new ConverterViewModel();

        Assert.Equal(UnitCategory.Length, viewModel.SelectedCategory);
        Assert.Equal("m", viewModel.FromUnit.Id);
        Assert.Equal("km", viewModel.ToUnit.Id);
    }

    [Theory]
    [InlineData(UnitCategory.Temperature, "c", "f")]
    [InlineData(UnitCategory.Speed, "kmh", "mph")]
    [InlineData(UnitCategory.Data, "gb", "gib")]
    [InlineData(UnitCategory.Angle, "deg", "rad")]
    public void ChangingCategory_AppliesDeterministicDefaults(UnitCategory category, string fromId, string toId)
    {
        var viewModel = new ConverterViewModel();

        viewModel.SelectedCategory = category;

        Assert.Equal(fromId, viewModel.FromUnit.Id);
        Assert.Equal(toId, viewModel.ToUnit.Id);
        Assert.Equal(category, viewModel.CurrentPair.Category);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void RestoringExplicitPair_StillOverridesCategoryDefaults()
    {
        var viewModel = new ConverterViewModel();
        var pair = new ConversionPair("kg", "g");

        viewModel.ApplyPairCommand.Execute(pair);

        Assert.Equal(UnitCategory.Mass, viewModel.SelectedCategory);
        Assert.Equal("kg", viewModel.FromUnit.Id);
        Assert.Equal("g", viewModel.ToUnit.Id);
    }
}
