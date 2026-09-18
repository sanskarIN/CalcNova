using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
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
    public async Task CalculatorKeypad_ReplacesTrackedSelectionAndRestoresCaret()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.Calculator.Expression = "12345";
            var expressionBox = view.GetVisualDescendants()
                .OfType<TextBox>()
                .First(textBox => ReferenceEquals(textBox.DataContext, viewModel.Calculator));
            expressionBox.SelectionStart = 1;
            expressionBox.SelectionEnd = 4;
            viewModel.Calculator.UpdateSelection(expressionBox.SelectionStart, expressionBox.SelectionEnd);

            var nineButton = view.GetVisualDescendants()
                .OfType<Button>()
                .First(button => string.Equals(button.Content?.ToString(), "9", StringComparison.Ordinal));
            Assert.NotNull(nineButton.Command);
            nineButton.Command.Execute(nineButton.CommandParameter);

            Assert.Equal("195", viewModel.Calculator.Expression);
            Assert.Equal(2, expressionBox.SelectionStart);
            Assert.Equal(2, expressionBox.SelectionEnd);
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
    public async Task GraphMode_SurfacesInteractivePlotAndTracksSampledSegments()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.SelectMode(7);
            Dispatcher.UIThread.RunJobs();

            var plot = view.GetVisualDescendants().OfType<GraphPlotControl>().Single();
            Assert.True(plot.Focusable);
            Assert.Equal(viewModel.Graphing.Segments.Count, plot.Segments?.Count);

            viewModel.Graphing.Expression = "x * x";
            viewModel.Graphing.PlotCommand.Execute(null);

            Assert.Same(viewModel.Graphing.Segments, plot.Segments);
            Assert.NotEmpty(plot.Segments ?? []);
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
    public async Task HindiCulture_LocalizesShellHeadersAndCalculatorPrompt()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.Settings.CultureName = "hi-IN";
            await viewModel.Settings.SaveAsync();

            var tabs = view.GetVisualDescendants().OfType<TabItem>().ToArray();
            Assert.Equal("कैलकुलेटर", tabs[0].Header?.ToString());
            Assert.Equal("परिचय", tabs[^1].Header?.ToString());
            Assert.Contains(
                view.GetVisualDescendants().OfType<TextBlock>(),
                textBlock => string.Equals(textBlock.Text, "मानक + वैज्ञानिक", StringComparison.Ordinal));

            var expressionBox = view.GetVisualDescendants()
                .OfType<TextBox>()
                .First(textBox => ReferenceEquals(textBox.DataContext, viewModel.Calculator));
            Assert.Equal("अभिव्यक्ति दर्ज करें", expressionBox.PlaceholderText);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HindiCulture_LocalizesVisibleOnboardingCopy()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 720, Height = 760, Content = view };

        window.Show();
        try
        {
            viewModel.Settings.CultureName = "hi-IN";
            await viewModel.Settings.SaveAsync();

            var overlay = view.GetVisualDescendants().OfType<OnboardingOverlay>().Single();
            Assert.True(overlay.IsVisible);
            Assert.Contains(
                overlay.GetVisualDescendants().OfType<TextBlock>(),
                textBlock => string.Equals(textBlock.Text, "CalcNova में आपका स्वागत है", StringComparison.Ordinal));
            Assert.Contains(
                overlay.GetVisualDescendants().OfType<Button>(),
                button => string.Equals(button.Content?.ToString(), "छोड़ें", StringComparison.Ordinal));
            Assert.Contains(
                overlay.GetVisualDescendants().OfType<Button>(),
                button => string.Equals(button.Content?.ToString(), "गणना शुरू करें", StringComparison.Ordinal));
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

    [AvaloniaFact]
    public async Task CompactShell_KeepsModeContentInsideTheWindow()
    {
        var viewModel = await CreateReadyViewModelAsync();
        var view = new MainView { DataContext = viewModel };

        // Below AdaptiveLayoutProfile.CompactMaximumWidth, which is where a phone lands.
        var window = new Window { Width = 420, Height = 860, Content = view };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            // A mode's content pane that scrolls horizontally is measured with unlimited width,
            // and then nothing inside it sizes itself to the window: WrapPanels stop wrapping and
            // star-sized keypad columns stretch past the edge, so the shell has to be panned to
            // reach a key. Only the strip that holds the mode tabs may scroll sideways.
            var contentPanes = view.GetVisualDescendants()
                .OfType<ScrollViewer>()
                .Where(scrollViewer => !scrollViewer.GetVisualDescendants().OfType<TabItem>().Any())
                .ToArray();

            Assert.NotEmpty(contentPanes);
            Assert.All(
                contentPanes,
                pane => Assert.Equal(ScrollBarVisibility.Disabled, pane.HorizontalScrollBarVisibility));

            // The contract above is what stops a pane overflowing; this is the overflow itself.
            Assert.All(
                contentPanes,
                pane => Assert.True(
                    pane.Extent.Width <= pane.Viewport.Width + 0.5,
                    $"A mode content pane measured {pane.Extent.Width} wide "
                        + $"inside a {pane.Viewport.Width} wide viewport."));
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
