using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class MainViewHeadlessTests
{
    [AvaloniaFact]
    public async Task SharedShell_LoadsEveryPrimaryMode()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            var tabs = view.GetVisualDescendants().OfType<TabItem>().ToArray();
            Assert.Equal(MainViewModel.ModeCount, tabs.Length);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task CalculatorClearButton_ExecutesBoundCommand()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.Calculator.Expression = "12345";
            var clearButton = view.GetVisualDescendants()
                .OfType<Button>()
                .First(button => string.Equals(button.Content?.ToString(), "AC", StringComparison.Ordinal));

            Assert.NotNull(clearButton.Command);
            Assert.True(clearButton.Command.CanExecute(clearButton.CommandParameter));
            clearButton.Command.Execute(clearButton.CommandParameter);

            Assert.Equal(string.Empty, viewModel.Calculator.Expression);
            Assert.Equal("0", viewModel.Calculator.Result);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task CompactWindow_AppliesCompactAdaptiveClass()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 480, Height = 760, Content = view };

        window.Show();
        try
        {
            Assert.Contains("compact", view.Classes);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task CtrlPageDown_AdvancesSharedModeSelection()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            var firstTab = view.GetVisualDescendants().OfType<TabItem>().First();
            Assert.True(firstTab.Focus());
            Assert.Equal(0, viewModel.SelectedModeIndex);

            window.KeyPressQwerty(PhysicalKey.PageDown, RawInputModifiers.Control);

            Assert.Equal(1, viewModel.SelectedModeIndex);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HighContrastPreference_AppliesShellClass()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.Settings.HighContrast = true;
            await viewModel.Settings.SaveAsync();

            Assert.Contains("high-contrast", view.Classes);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task NewUser_OnboardingOverlayIsVisibleAndSkipHidesIt()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 720, Height = 760, Content = view };

        window.Show();
        try
        {
            var overlay = view.GetVisualDescendants().OfType<OnboardingOverlay>().Single();
            Assert.True(overlay.IsVisible);

            await viewModel.Settings.SkipOnboardingAsync();

            Assert.False(viewModel.Settings.ShouldShowOnboarding);
            Assert.False(overlay.IsVisible);
        }
        finally
        {
            window.Close();
        }
    }

    private static async Task<MainViewModel> CreateReadyViewModelAsync()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        return viewModel;
    }
}
