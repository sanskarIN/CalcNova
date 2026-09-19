using Avalonia;
using Avalonia.Automation;
using Avalonia.Media;
using Avalonia.Styling;
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

            // A mode's content pane measured with unlimited width leaves nothing inside it able
            // to fit the window: WrapPanels such as the programmer bit grid stop wrapping and run
            // off in one line, and star-sized keypad columns stretch to the widest label in the
            // pane, so keys sit off-screen and the shell has to be panned to reach them.
            //
            // This deliberately reads the panes off the tabs rather than collecting every
            // ScrollViewer in the shell. A TextBox carries a ScrollViewer inside its own
            // template, and that one should keep scrolling a long expression sideways.
            var contentPanes = view.GetVisualDescendants()
                .OfType<TabItem>()
                .Select(tab => tab.Content)
                .OfType<ScrollViewer>()
                .ToArray();

            Assert.NotEmpty(contentPanes);
            Assert.All(
                contentPanes,
                pane => Assert.Equal(ScrollBarVisibility.Disabled, pane.HorizontalScrollBarVisibility));

            // Only the selected mode is laid out, so only that pane reports a real viewport.
            var laidOut = contentPanes.Where(pane => pane.Viewport.Width > 0).ToArray();
            Assert.NotEmpty(laidOut);
            Assert.All(
                laidOut,
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

    // 360x800 is the phone the overlay was reported unusable on. 360x640 is a shorter phone
    // still, and 800x360 is that phone in landscape, where the card has least height of all.
    [AvaloniaTheory]
    [InlineData(360, 800)]
    [InlineData(360, 640)]
    [InlineData(800, 360)]
    public async Task OnboardingActions_StayOnScreen(double width, double height)
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        var view = new MainView { DataContext = viewModel };

        // Where the overlay ran out of room: the card was taller than the screen, and Skip and
        // Start calculating sat at the bottom of the scrolled content, so onboarding could not
        // be dismissed and the app could not be reached.
        var window = new Window { Width = width, Height = height, Content = view };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var overlay = view.GetVisualDescendants().OfType<OnboardingOverlay>().Single();
            Assert.True(overlay.IsVisible);

            foreach (var name in new[]
                     {
                         "Skip CalcNova introduction",
                         "Complete CalcNova introduction and start calculating",
                     })
            {
                var button = overlay.GetVisualDescendants()
                    .OfType<Button>()
                    .Single(candidate => string.Equals(
                        AutomationProperties.GetName(candidate),
                        name,
                        StringComparison.Ordinal));

                Assert.True(button.IsVisible);
                Assert.True(button.Bounds.Width > 0 && button.Bounds.Height > 0, $"'{name}' has no size.");

                // Bounds are parent-relative, so the corners have to be translated into the
                // window before they mean anything.
                var topLeft = button.TranslatePoint(new Point(0, 0), window);
                var bottomRight = button.TranslatePoint(
                    new Point(button.Bounds.Width, button.Bounds.Height),
                    window);

                Assert.NotNull(topLeft);
                Assert.NotNull(bottomRight);
                Assert.True(
                    topLeft!.Value.Y >= 0 && bottomRight!.Value.Y <= window.Height,
                    $"'{name}' spans {topLeft.Value.Y} to {bottomRight!.Value.Y} in an {window.Height} tall window.");
                Assert.True(
                    topLeft.Value.X >= 0 && bottomRight.Value.X <= window.Width,
                    $"'{name}' spans {topLeft.Value.X} to {bottomRight.Value.X} in a {window.Width} wide window.");
            }
        }
        finally
        {
            window.Close();
        }
    }

    // The card paints a fixed light surface. Buttons that follow the app's theme variant instead
    // render near-white text on it in dark mode - present and tappable, but invisible, which is
    // indistinguishable from onboarding having no way out.
    [AvaloniaTheory]
    [InlineData("Dark")]
    [InlineData("Light")]
    public async Task OnboardingActions_StayLegibleAgainstTheCard(string variantName)
    {
        var application = Application.Current;
        Assert.NotNull(application);
        var previousVariant = application!.RequestedThemeVariant;
        application.RequestedThemeVariant =
            string.Equals(variantName, "Dark", StringComparison.Ordinal) ? ThemeVariant.Dark : ThemeVariant.Light;

        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 360, Height = 800, Content = view };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var overlay = view.GetVisualDescendants().OfType<OnboardingOverlay>().Single();

            foreach (var name in new[]
                     {
                         "Skip CalcNova introduction",
                         "Complete CalcNova introduction and start calculating",
                     })
            {
                var button = overlay.GetVisualDescendants()
                    .OfType<Button>()
                    .Single(candidate => string.Equals(
                        AutomationProperties.GetName(candidate),
                        name,
                        StringComparison.Ordinal));

                var foreground = Assert.IsAssignableFrom<ISolidColorBrush>(button.Foreground).Color;
                var behind = NearestOpaqueBackground(button);

                var contrast = ContrastRatio(foreground, behind);
                Assert.True(
                    contrast >= 4.5,
                    $"'{name}' in {variantName}: {foreground} on {behind} is only {contrast:F2}:1.");
            }
        }
        finally
        {
            window.Close();
            application.RequestedThemeVariant = previousVariant;
        }
    }

    /// <summary>The colour actually behind a control: its own button face, else the card.</summary>
    private static Color NearestOpaqueBackground(Button button)
    {
        if (button.Background is ISolidColorBrush { Color.A: 255 } own)
        {
            return own.Color;
        }

        foreach (var ancestor in button.GetVisualAncestors().OfType<Border>())
        {
            if (ancestor.Background is ISolidColorBrush { Color.A: 255 } solid)
            {
                return solid.Color;
            }
        }

        throw new InvalidOperationException("No opaque surface was found behind the button.");
    }

    private static double ContrastRatio(Color first, Color second)
    {
        var a = RelativeLuminance(first);
        var b = RelativeLuminance(second);
        var lighter = Math.Max(a, b);
        var darker = Math.Min(a, b);
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double RelativeLuminance(Color color)
    {
        static double Channel(byte value)
        {
            var c = value / 255d;
            return c <= 0.03928d ? c / 12.92d : Math.Pow((c + 0.055d) / 1.055d, 2.4d);
        }

        return (0.2126d * Channel(color.R)) + (0.7152d * Channel(color.G)) + (0.0722d * Channel(color.B));
    }

    private static async Task<MainViewModel> CreateReadyViewModelAsync()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        return viewModel;
    }
}
