using CalcNova.App.ViewModels;
using CalcNova.Converter;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterViewModelTests
{
    [Fact]
    public void UnitDefinition_ToString_IsHumanReadable()
    {
        var metre = UnitCatalog.ForCategory(UnitCategory.Length).First(unit => unit.Id == "m");

        Assert.Equal("Metre (m)", metre.ToString());
    }

    [Fact]
    public void Convert_AddsCurrentPairToRecentList()
    {
        var viewModel = new ConverterViewModel();
        var from = viewModel.AvailableUnits.First();
        var to = viewModel.AvailableUnits.Skip(1).First();
        viewModel.FromUnit = from;
        viewModel.ToUnit = to;
        viewModel.Input = "12";

        viewModel.Convert();

        var recent = Assert.Single(viewModel.RecentPairs.Where(pair => pair.From.Id == from.Id && pair.To.Id == to.Id));
        Assert.Equal(recent, viewModel.SelectedRecentPair);
    }

    [Fact]
    public void FavoriteCommand_TogglesCurrentPair()
    {
        var viewModel = new ConverterViewModel();

        viewModel.ToggleFavoriteCommand.Execute(null);
        Assert.True(viewModel.IsCurrentPairFavorite);
        Assert.Single(viewModel.FavoritePairs);

        viewModel.ToggleFavoriteCommand.Execute(null);
        Assert.False(viewModel.IsCurrentPairFavorite);
        Assert.Empty(viewModel.FavoritePairs);
    }

    [Fact]
    public void UseFavorite_RestoresPairAfterChangingCategory()
    {
        var viewModel = new ConverterViewModel();
        var expectedFrom = viewModel.FromUnit;
        var expectedTo = viewModel.ToUnit;
        viewModel.ToggleFavoriteCommand.Execute(null);
        viewModel.SelectedFavoritePair = Assert.Single(viewModel.FavoritePairs);
        viewModel.SelectedCategory = UnitCategory.Temperature;

        viewModel.UseFavoriteCommand.Execute(null);

        Assert.Equal(expectedFrom.Id, viewModel.FromUnit.Id);
        Assert.Equal(expectedTo.Id, viewModel.ToUnit.Id);
        Assert.Equal(UnitCategory.Length, viewModel.SelectedCategory);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void RecentPairs_AreBoundedAndDeduplicated()
    {
        var viewModel = new ConverterViewModel();
        var categories = Enum.GetValues<UnitCategory>();

        foreach (var category in categories.Take(10))
        {
            viewModel.SelectedCategory = category;
            var units = viewModel.AvailableUnits;
            if (units.Count < 2)
            {
                continue;
            }

            viewModel.FromUnit = units[0];
            viewModel.ToUnit = units[1];
            viewModel.Convert();
            viewModel.Convert();
        }

        Assert.True(viewModel.RecentPairs.Count <= 8);
        Assert.Equal(
            viewModel.RecentPairs.Count,
            viewModel.RecentPairs.Select(pair => (pair.From.Id, pair.To.Id)).Distinct().Count());
    }
}
