using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CalcNova.App.Infrastructure;
using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Views;

public partial class MainView : UserControl
{
    private static readonly string[] AdaptiveStyleClasses = ["compact", "medium", "expanded"];

    private MainViewModel? _subscribedViewModel;

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
                _subscribedViewModel.SettingsChanged -= ApplySettings;
            }

            _subscribedViewModel = viewModel;
            viewModel.SettingsChanged += ApplySettings;
        }

        await viewModel.InitializeAsync();
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs eventArgs)
    {
        if (AppComposition.Dependencies.ClipboardService is AvaloniaClipboardService clipboardService)
        {
            clipboardService.Attach(null);
        }

        if (_subscribedViewModel is null)
        {
            return;
        }

        _subscribedViewModel.SettingsChanged -= ApplySettings;
        _subscribedViewModel = null;
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

        if ((eventArgs.KeyModifiers & KeyModifiers.Control) != 0)
        {
            switch (eventArgs.Key)
            {
                case Key.PageDown:
                    viewModel.SelectNextMode();
                    eventArgs.Handled = true;
                    return;
                case Key.PageUp:
                    viewModel.SelectPreviousMode();
                    eventArgs.Handled = true;
                    return;
            }
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
