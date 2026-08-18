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

        var recent = Assert.Single(viewModel.RecentPairs, pair => pair.From.Id == from.Id && pair.To.Id == to.Id);
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

    [Fact]
    public void PrecisionOptions_CoverSupportedDisplayRange()
    {
        var viewModel = new ConverterViewModel();

        Assert.Equal(3, viewModel.PrecisionOptions[0]);
        Assert.Equal(15, viewModel.PrecisionOptions[^1]);
        Assert.Equal(13, viewModel.PrecisionOptions.Count);
    }

    [Fact]
    public void Precision_ChangingValueReformatsCurrentConversion()
    {
        var viewModel = new ConverterViewModel
        {
            Input = "1.23456789"
        };
        viewModel.ToUnit = viewModel.FromUnit;
        viewModel.Precision = 4;

        Assert.StartsWith("1.235 ", viewModel.Result, StringComparison.Ordinal);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(16)]
    public void Precision_OutOfRangeValueIsIgnored(int invalidPrecision)
    {
        var viewModel = new ConverterViewModel();
        var originalPrecision = viewModel.Precision;

        viewModel.Precision = invalidPrecision;

        Assert.Equal(originalPrecision, viewModel.Precision);
    }
}
