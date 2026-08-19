using CalcNova.App.ViewModels;
using CalcNova.Converter;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterSelectionViewModelTests
{
    [Fact]
    public void SelectedPair_AppliesPairAndClearsSelectionForReselection()
    {
        var viewModel = new ConverterViewModel();
        var pair = new ConversionPair("kg", "g");

        viewModel.SelectedPair = pair;

        Assert.Equal(UnitCategory.Mass, viewModel.SelectedCategory);
        Assert.Equal("kg", viewModel.FromUnit.Id);
        Assert.Equal("g", viewModel.ToUnit.Id);
        Assert.Null(viewModel.SelectedPair);
    }

    [Fact]
    public void FavoriteToggleLabel_TracksCurrentPairState()
    {
        var viewModel = new ConverterViewModel();

        Assert.Equal("Add favorite", viewModel.FavoriteToggleLabel);

        viewModel.ToggleFavoriteCommand.Execute(null);

        Assert.Equal("Remove favorite", viewModel.FavoriteToggleLabel);
    }
}
