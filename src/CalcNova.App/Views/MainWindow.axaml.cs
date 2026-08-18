using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using CalcNova.App.ViewModels;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Views;

public partial class MainWindow : Window
{
    private MainViewModel? _subscribedViewModel;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private async void OnOpened(object? sender, EventArgs eventArgs)
    {
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

    private void OnClosed(object? sender, EventArgs eventArgs)
    {
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

        var calculator = viewModel.Calculator;
        switch (eventArgs.Key)
        {
            case Key.Enter:
                await calculator.EvaluateAsync();
                eventArgs.Handled = true;
                break;
            case Key.Escape:
                calculator.Clear();
                eventArgs.Handled = true;
                break;
            case Key.Back:
                calculator.Backspace();
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
