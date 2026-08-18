using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnKeyDown(object? sender, KeyEventArgs eventArgs)
    {
        if (DataContext is not MainViewModel viewModel || viewModel.SelectedModeIndex != 0)
        {
            return;
        }

        var calculator = viewModel.Calculator;
        switch (eventArgs.Key)
        {
            case Key.Enter:
                calculator.Evaluate();
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
}
