using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Views;

public partial class MainView : UserControl
{
    private MainViewModel? _subscribedViewModel;

    public MainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs eventArgs)
    {
        if (AppComposition.Dependencies.ClipboardService is AvaloniaClipboardService clipboardService)
        {
            clipboardService.Attach(TopLevel.GetTopLevel(this)?.Clipboard);
        }

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
        if (DataContext is not MainViewModel viewModel || viewModel.SelectedModeIndex != 0)
        {
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
            case Key.Back:
                viewModel.Calculator.Backspace();
                eventArgs.Handled = true;
                break;
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
