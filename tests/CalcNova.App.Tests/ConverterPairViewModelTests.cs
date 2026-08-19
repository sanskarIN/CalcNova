using CalcNova.App.ViewModels;
using CalcNova.Converter;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterPairViewModelTests
{
    [Fact]
    public void SuccessfulConversion_RecordsCurrentPairAsMostRecent()
    {
        var viewModel = new ConverterViewModel
        {
            SelectedCategory = UnitCategory.Length,
            Input = "2"
        };
        viewModel.FromUnit = viewModel.AvailableUnits.Single(unit => unit.Id == "km");
        viewModel.ToUnit = viewModel.AvailableUnits.Single(unit => unit.Id == "m");

        viewModel.ConvertCommand.Execute(null);

        Assert.Equal(new ConversionPair("km", "m"), viewModel.RecentPairs[0]);
    }

    [Fact]
    public void ToggleFavoriteCommand_TogglesCurrentPair()
    {
        var viewModel = new ConverterViewModel();

        viewModel.ToggleFavoriteCommand.Execute(null);

        Assert.True(viewModel.IsCurrentPairFavorite);
        Assert.Contains(viewModel.CurrentPair, viewModel.FavoritePairs);

        viewModel.ToggleFavoriteCommand.Execute(null);

        Assert.False(viewModel.IsCurrentPairFavorite);
        Assert.DoesNotContain(viewModel.CurrentPair, viewModel.FavoritePairs);
    }

    [Fact]
    public void ApplyPairCommand_RestoresCategoryAndUnits()
    {
        var viewModel = new ConverterViewModel();
        var pair = new ConversionPair("kg", "g");

        viewModel.ApplyPairCommand.Execute(pair);

        Assert.Equal(UnitCategory.Mass, viewModel.SelectedCategory);
        Assert.Equal("kg", viewModel.FromUnit.Id);
        Assert.Equal("g", viewModel.ToUnit.Id);
        Assert.Equal(pair, viewModel.RecentPairs[0]);
    }
}
