using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class MainViewModelNavigationTests
{
    [Fact]
    public void SelectNextMode_AdvancesSelection()
    {
        var viewModel = new MainViewModel { SelectedModeIndex = 4 };

        viewModel.SelectNextMode();

        Assert.Equal(5, viewModel.SelectedModeIndex);
    }

    [Fact]
    public void SelectNextMode_WrapsFromLastModeToFirst()
    {
        var viewModel = new MainViewModel { SelectedModeIndex = MainViewModel.ModeCount - 1 };

        viewModel.SelectNextMode();

        Assert.Equal(0, viewModel.SelectedModeIndex);
    }

    [Fact]
    public void SelectPreviousMode_MovesSelectionBackward()
    {
        var viewModel = new MainViewModel { SelectedModeIndex = 4 };

        viewModel.SelectPreviousMode();

        Assert.Equal(3, viewModel.SelectedModeIndex);
    }

    [Fact]
    public void SelectPreviousMode_WrapsFromFirstModeToLast()
    {
        var viewModel = new MainViewModel { SelectedModeIndex = 0 };

        viewModel.SelectPreviousMode();

        Assert.Equal(MainViewModel.ModeCount - 1, viewModel.SelectedModeIndex);
    }
}
