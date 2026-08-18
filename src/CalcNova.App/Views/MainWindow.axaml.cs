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
        if (DataContext is not CalculatorViewModel viewModel)
        {
            return;
        }

        switch (eventArgs.Key)
        {
            case Key.Enter:
                viewModel.Evaluate();
                eventArgs.Handled = true;
                break;
            case Key.Escape:
                viewModel.Clear();
                eventArgs.Handled = true;
                break;
            case Key.Back:
                viewModel.Backspace();
                eventArgs.Handled = true;
                break;
        }
    }
}
