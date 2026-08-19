using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.Infrastructure;
using CalcNova.App.Localization;
using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Graphing;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Views;

public partial class MainView : UserControl
{
    private static readonly string[] AdaptiveStyleClasses = ["compact", "medium", "expanded"];
    private static readonly string[] AccessibilityStyleClasses = ["high-contrast", "reduced-motion"];

    private readonly Dictionary<TextBlock, AppStringKey> _localizedTextBlocks = new();
    private readonly Dictionary<Button, AppStringKey> _localizedButtons = new();
    private readonly Dictionary<TextBox, AppStringKey> _localizedWatermarks = new();
    private MainViewModel? _subscribedViewModel;
    private MainViewModel? _localizationViewModel;
    private TabControl? _localizationTabControl;
    private TextBox? _calculatorExpressionTextBox;
    private CalculatorViewModel? _calculatorEditorViewModel;
    private GraphPlotControl? _graphPlotControl;
    private TextBlock? _graphLegendTextBlock;
    private GraphingViewModel? _graphPlotViewModel;
    private bool _onboardingWasVisible;

    public MainView()
    {
        InitializeComponent();
        AttachOnboardingOverlay();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void AttachOnboardingOverlay()
    {
        if (Content is not Grid shell)
        {
            return;
        }

        var overlay = new OnboardingOverlay();
        Grid.SetRowSpan(overlay, Math.Max(1, shell.RowDefinitions.Count));
        shell.Children.Add(overlay);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs eventArgs)
    {
        base.OnSizeChanged(eventArgs);

        if (eventArgs.WidthChanged)
        {
            ApplyAdaptiveLayout(eventArgs.NewSize.Width);
        }
    }

    private async void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs eventArgs)
    {
        if (AppComposition.Dependencies.ClipboardService is AvaloniaClipboardService clipboardService)
        {
            clipboardService.Attach(TopLevel.GetTopLevel(this)?.Clipboard);
        }

        ApplyAdaptiveLayout(Bounds.Width);

        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (!ReferenceEquals(_subscribedViewModel, viewModel))
        {
            if (_subscribedViewModel is not null)
            {
                _subscribedViewModel.SettingsChanged -= HandleSettingsChanged;
            }

            _subscribedViewModel = viewModel;
            viewModel.SettingsChanged += HandleSettingsChanged;
        }

        AttachLocalization(viewModel);
        AttachCalculatorExpressionEditor(viewModel.Calculator);
        EnsureGraphPlot(viewModel.Graphing);

        await viewModel.InitializeAsync();
        CaptureLocalizedControls();
        ApplyLocalization();
        EnsureGraphPlot(viewModel.Graphing);
        _onboardingWasVisible = viewModel.Settings.ShouldShowOnboarding;
        if (_onboardingWasVisible)
        {
            QueueOnboardingFocus();
        }
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs eventArgs)
    {
        if (AppComposition.Dependencies.ClipboardService is AvaloniaClipboardService clipboardService)
        {
            clipboardService.Attach(null);
        }

        DetachGraphPlot();
        DetachLocalization();
        DetachCalculatorExpressionEditor();

        if (_subscribedViewModel is null)
        {
            return;
        }

        _subscribedViewModel.SettingsChanged -= HandleSettingsChanged;
        _subscribedViewModel = null;
        _onboardingWasVisible = false;
    }

    private async void OnKeyDown(object? sender, KeyEventArgs eventArgs)
    {
        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (viewModel.Settings.ShouldShowOnboarding)
        {
            return;
        }

        var navigationAction = ShellKeyboardShortcut.GetNavigationAction(eventArgs.Key, eventArgs.KeyModifiers);
        if (navigationAction != ShellNavigationAction.None)
        {
            ApplyShellNavigation(viewModel, navigationAction);
            eventArgs.Handled = true;
            return;
        }

        if (viewModel.SelectedModeIndex != 0)
        {
            return;
        }

        if (eventArgs.Source is not TextBox &&
            eventArgs.KeyModifiers == KeyModifiers.None &&
            CalculatorKeyboardInput.TryGetToken(eventArgs.Key, out var token))
        {
            viewModel.Calculator.AppendCommand.Execute(token);
            eventArgs.Handled = true;
            return;
        }

        switch (eventArgs.Key)
        {
            case Key.Enter:
                await viewModel.Calculator.EvaluateAsync();
                eventArgs.Handled = true;
                break;
            case Key.Escape:
                viewModel.Calculator.Clear();
                eventArgs.Handled = true;
                break;
            case Key.Back when eventArgs.Source is not TextBox:
                viewModel.Calculator.Backspace();
                eventArgs.Handled = true;
                break;
        }
    }

    private static void ApplyShellNavigation(MainViewModel viewModel, ShellNavigationAction action)
    {
        switch (action)
        {
            case ShellNavigationAction.PreviousMode:
                viewModel.SelectPreviousMode();
                break;
            case ShellNavigationAction.NextMode:
                viewModel.SelectNextMode();
                break;
            case ShellNavigationAction.FirstMode:
                viewModel.SelectFirstMode();
                break;
            case ShellNavigationAction.LastMode:
                viewModel.SelectLastMode();
                break;
        }
    }

    private void AttachLocalization(MainViewModel viewModel)
    {
        DetachLocalization();

        _localizationViewModel = viewModel;
        viewModel.Localizer.CultureChanged += HandleCultureChanged;
        _localizationTabControl = this.GetVisualDescendants().OfType<TabControl>().FirstOrDefault();
        if (_localizationTabControl is not null)
        {
            _localizationTabControl.SelectionChanged += HandleLocalizationSelectionChanged;
        }

        CaptureLocalizedControls();
        ApplyLocalization();
    }

    private void DetachLocalization()
    {
        if (_localizationViewModel is not null)
        {
            _localizationViewModel.Localizer.CultureChanged -= HandleCultureChanged;
        }

        if (_localizationTabControl is not null)
        {
            _localizationTabControl.SelectionChanged -= HandleLocalizationSelectionChanged;
        }

        _localizationViewModel = null;
        _localizationTabControl = null;
        _localizedTextBlocks.Clear();
        _localizedButtons.Clear();
        _localizedWatermarks.Clear();
    }

    private void HandleCultureChanged(CultureInfo culture) => RefreshLocalizationTargets();

    private void HandleLocalizationSelectionChanged(object? sender, SelectionChangedEventArgs eventArgs)
    {
        RefreshLocalizationTargets();
        if (_subscribedViewModel is not null)
        {
            EnsureGraphPlot(_subscribedViewModel.Graphing);
        }
    }

    private void RefreshLocalizationTargets()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            CaptureLocalizedControls();
            ApplyLocalization();
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            CaptureLocalizedControls();
            ApplyLocalization();
        });
    }

    private void CaptureLocalizedControls()
    {
        foreach (var textBlock in this.GetVisualDescendants().OfType<TextBlock>())
        {
            if (!_localizedTextBlocks.ContainsKey(textBlock) &&
                ShellLocalization.TryGetLiteralKey(textBlock.Text, out var key))
            {
                _localizedTextBlocks[textBlock] = key;
            }
        }

        foreach (var button in this.GetVisualDescendants().OfType<Button>())
        {
            if (!_localizedButtons.ContainsKey(button) &&
                button.Content is string literal &&
                ShellLocalization.TryGetLiteralKey(literal, out var key))
            {
                _localizedButtons[button] = key;
            }
        }

        foreach (var textBox in this.GetVisualDescendants().OfType<TextBox>())
        {
            if (!_localizedWatermarks.ContainsKey(textBox) &&
                textBox.Watermark is string literal &&
                ShellLocalization.TryGetLiteralKey(literal, out var key))
            {
                _localizedWatermarks[textBox] = key;
            }
        }
    }

    private void ApplyLocalization()
    {
        var localizer = _localizationViewModel?.Localizer;
        if (localizer is null)
        {
            return;
        }

        foreach (var (textBlock, key) in _localizedTextBlocks)
        {
            textBlock.Text = localizer[key];
        }

        foreach (var (button, key) in _localizedButtons)
        {
            button.Content = localizer[key];
        }

        foreach (var (textBox, key) in _localizedWatermarks)
        {
            textBox.Watermark = localizer[key];
        }

        var modeHeaders = ShellLocalization.GetModeHeaders(localizer);
        var tabs = this.GetVisualDescendants().OfType<TabItem>().Take(modeHeaders.Count).ToArray();
        for (var index = 0; index < tabs.Length; index++)
        {
            tabs[index].Header = modeHeaders[index];
        }
    }

    private void EnsureGraphPlot(GraphingViewModel graphing)
    {
        if (_graphPlotControl is not null && ReferenceEquals(_graphPlotViewModel, graphing))
        {
            SynchronizeGraphPlot();
            return;
        }

        var graphPanel = this.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(panel => ReferenceEquals(panel.DataContext, graphing));
        if (graphPanel is null)
        {
            return;
        }

        DetachGraphPlot();

        var plot = new GraphPlotControl
        {
            MinHeight = 300,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        ToolTip.SetTip(plot, "Interactive graph: drag to pan, wheel or numpad +/- to zoom, Home to reset, F to fit data.");

        var legend = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.72,
            IsVisible = false,
            Margin = new Thickness(0, 2, 0, 4)
        };
        legend.Classes.Add("graph-series-legend");

        var insertionIndex = Math.Min(8, graphPanel.Children.Count);
        graphPanel.Children.Insert(insertionIndex, plot);
        graphPanel.Children.Insert(Math.Min(insertionIndex + 1, graphPanel.Children.Count), legend);
        _graphPlotControl = plot;
        _graphLegendTextBlock = legend;
        _graphPlotViewModel = graphing;
        graphing.PropertyChanged += HandleGraphingPropertyChanged;
        SynchronizeGraphPlot();
    }

    private void DetachGraphPlot()
    {
        if (_graphPlotViewModel is not null)
        {
            _graphPlotViewModel.PropertyChanged -= HandleGraphingPropertyChanged;
        }

        if (_graphPlotControl?.Parent is Panel plotPanel)
        {
            plotPanel.Children.Remove(_graphPlotControl);
        }

        if (_graphLegendTextBlock?.Parent is Panel legendPanel)
        {
            legendPanel.Children.Remove(_graphLegendTextBlock);
        }

        _graphPlotControl = null;
        _graphLegendTextBlock = null;
        _graphPlotViewModel = null;
    }

    private void HandleGraphingPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is nameof(GraphingViewModel.Segments)
            or nameof(GraphingViewModel.MultiSeries)
            or nameof(GraphingViewModel.PlotMode))
        {
            SynchronizeGraphPlot();
        }
    }

    private void SynchronizeGraphPlot()
    {
        if (_graphPlotControl is null || _graphPlotViewModel is null)
        {
            return;
        }

        if (_graphPlotViewModel.PlotMode == GraphPlotMode.Multiple && _graphPlotViewModel.MultiSeries.Count > 0)
        {
            _graphPlotControl.Segments = Array.Empty<GraphSegment>();
            _graphPlotControl.Series = _graphPlotViewModel.MultiSeries;

            if (_graphLegendTextBlock is not null)
            {
                var presentations = GraphSeriesPresentationFactory.Create(_graphPlotViewModel.MultiSeries);
                _graphLegendTextBlock.Text = string.Join(
                    Environment.NewLine,
                    presentations.Select(presentation => presentation.LegendText));
                _graphLegendTextBlock.IsVisible = presentations.Count > 0;
            }

            return;
        }

        _graphPlotControl.Series = Array.Empty<GraphExpressionSample>();
        _graphPlotControl.Segments = _graphPlotViewModel.Segments;
        if (_graphLegendTextBlock is not null)
        {
            _graphLegendTextBlock.Text = string.Empty;
            _graphLegendTextBlock.IsVisible = false;
        }
    }

    private void AttachCalculatorExpressionEditor(CalculatorViewModel calculator)
    {
        DetachCalculatorExpressionEditor();

        var textBox = this.GetVisualDescendants()
            .OfType<TextBox>()
            .FirstOrDefault(candidate => ReferenceEquals(candidate.DataContext, calculator));
        if (textBox is null)
        {
            return;
        }

        _calculatorExpressionTextBox = textBox;
        _calculatorEditorViewModel = calculator;
        textBox.KeyUp += HandleCalculatorExpressionKeyUp;
        textBox.PointerReleased += HandleCalculatorExpressionPointerReleased;
        calculator.SelectionRequested += HandleCalculatorSelectionRequested;
        SynchronizeCalculatorSelection();
    }

    private void DetachCalculatorExpressionEditor()
    {
        if (_calculatorExpressionTextBox is not null)
        {
            _calculatorExpressionTextBox.KeyUp -= HandleCalculatorExpressionKeyUp;
            _calculatorExpressionTextBox.PointerReleased -= HandleCalculatorExpressionPointerReleased;
        }

        if (_calculatorEditorViewModel is not null)
        {
            _calculatorEditorViewModel.SelectionRequested -= HandleCalculatorSelectionRequested;
        }

        _calculatorExpressionTextBox = null;
        _calculatorEditorViewModel = null;
    }

    private void HandleCalculatorExpressionKeyUp(object? sender, KeyEventArgs eventArgs) =>
        SynchronizeCalculatorSelection();

    private void HandleCalculatorExpressionPointerReleased(object? sender, PointerReleasedEventArgs eventArgs) =>
        SynchronizeCalculatorSelection();

    private void SynchronizeCalculatorSelection()
    {
        if (_calculatorExpressionTextBox is null || _calculatorEditorViewModel is null)
        {
            return;
        }

        _calculatorEditorViewModel.UpdateSelection(
            _calculatorExpressionTextBox.SelectionStart,
            _calculatorExpressionTextBox.SelectionEnd);
    }

    private void HandleCalculatorSelectionRequested(int selectionStart, int selectionEnd)
    {
        if (_calculatorExpressionTextBox is null)
        {
            return;
        }

        _calculatorExpressionTextBox.SelectionStart = selectionStart;
        _calculatorExpressionTextBox.SelectionEnd = selectionEnd;
    }

    private void HandleSettingsChanged(AppSettings settings)
    {
        ApplySettings(settings);
        ApplyAccessibilityPreferences(settings);

        var onboardingIsVisible = _subscribedViewModel?.Settings.ShouldShowOnboarding ?? false;
        if (_onboardingWasVisible && !onboardingIsVisible)
        {
            QueueCalculatorFocus();
        }

        _onboardingWasVisible = onboardingIsVisible;
    }

    private void ApplyAccessibilityPreferences(AppSettings settings)
    {
        foreach (var styleClass in AccessibilityStyleClasses)
        {
            Classes.Remove(styleClass);
        }

        if (settings.HighContrast)
        {
            Classes.Add("high-contrast");
        }

        if (settings.ReducedMotion)
        {
            Classes.Add("reduced-motion");
        }
    }

    private void QueueOnboardingFocus()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var overlay = this.GetVisualDescendants().OfType<OnboardingOverlay>().FirstOrDefault();
            overlay?.GetVisualDescendants().OfType<Button>().FirstOrDefault()?.Focus();
        });
    }

    private void QueueCalculatorFocus()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var calculator = _subscribedViewModel?.Calculator;
            this.GetVisualDescendants()
                .OfType<TextBox>()
                .FirstOrDefault(textBox => ReferenceEquals(textBox.DataContext, calculator))
                ?.Focus();
        });
    }

    private void ApplyAdaptiveLayout(double width)
    {
        var profile = AdaptiveLayoutProfile.ForWidth(width);

        foreach (var styleClass in AdaptiveStyleClasses)
        {
            Classes.Remove(styleClass);
        }

        Classes.Add(profile.StyleClass);

        var shell = this.GetVisualChildren().OfType<Grid>().FirstOrDefault();
        if (shell is not null)
        {
            shell.Margin = new Thickness(profile.ShellMargin);
        }

        var horizontalVisibility = profile.AllowHorizontalModeScrolling
            ? ScrollBarVisibility.Auto
            : ScrollBarVisibility.Disabled;

        foreach (var scrollViewer in this.GetVisualDescendants().OfType<ScrollViewer>())
        {
            scrollViewer.HorizontalScrollBarVisibility = horizontalVisibility;
            scrollViewer.BringIntoViewOnFocusChange = true;
        }
    }

    private static void ApplySettings(AppSettings settings)
    {
        if (Application.Current is null)
        {
            return;
        }

        Application.Current.RequestedThemeVariant = settings.Theme switch
        {
            ThemePreference.Light => ThemeVariant.Light,
            ThemePreference.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}
