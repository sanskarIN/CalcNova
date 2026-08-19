using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterSearchViewModelTests
{
    [Fact]
    public void UnitSearchQuery_FiltersCurrentCategory()
    {
        var viewModel = new ConverterViewModel
        {
            UnitSearchQuery = "meter"
        };

        Assert.Contains(viewModel.SearchResults, unit => unit.Id == "m");
        Assert.All(viewModel.SearchResults, unit => Assert.Equal(viewModel.SelectedCategory, unit.Category));
    }

    [Fact]
    public void UseSearchAsFromCommand_AppliesSelectedSearchUnit()
    {
        var viewModel = new ConverterViewModel
        {
            UnitSearchQuery = "kilometer"
        };
        viewModel.SelectedSearchUnit = viewModel.SearchResults.Single(unit => unit.Id == "km");

        viewModel.UseSearchAsFromCommand.Execute(null);

        Assert.Equal("km", viewModel.FromUnit.Id);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void UseSearchAsToCommand_RequiresSelection()
    {
        var viewModel = new ConverterViewModel();

        viewModel.UseSearchAsToCommand.Execute(null);

        Assert.Contains("select", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
