using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.VisualTree;
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
                return;
            case Key.Escape:
                calculator.Clear();
                eventArgs.Handled = true;
                return;
        }

        if (eventArgs.Source is TextBox)
        {
            return;
        }

        if (eventArgs.Key == Key.Back)
        {
            calculator.Backspace();
            eventArgs.Handled = true;
            return;
        }

        var token = GetCalculatorToken(eventArgs);
        if (token is null)
        {
            return;
        }

        calculator.AppendCommand.Execute(token);
        eventArgs.Handled = true;
    }

    private static string? GetCalculatorToken(KeyEventArgs eventArgs)
    {
        if (eventArgs.KeySymbol is { Length: 1 } symbol && "0123456789.+-*/^()%".Contains(symbol, StringComparison.Ordinal))
        {
            return symbol;
        }

        return eventArgs.Key switch
        {
            Key.NumPad0 => "0",
            Key.NumPad1 => "1",
            Key.NumPad2 => "2",
            Key.NumPad3 => "3",
            Key.NumPad4 => "4",
            Key.NumPad5 => "5",
            Key.NumPad6 => "6",
            Key.NumPad7 => "7",
            Key.NumPad8 => "8",
            Key.NumPad9 => "9",
            Key.Add => "+",
            Key.Subtract => "-",
            Key.Multiply => "*",
            Key.Divide => "/",
            Key.Decimal => ".",
            _ => null
        };
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
