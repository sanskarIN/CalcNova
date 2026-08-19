using Avalonia.Controls;
using Avalonia.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views;

public partial class MainView
{
    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        if (e.Handled ||
            DataContext is not MainViewModel viewModel ||
            viewModel.Settings.ShouldShowOnboarding ||
            viewModel.SelectedModeIndex != 0 ||
            e.Source is TextBox ||
            !CalculatorTextSymbolInput.TryGetToken(e.Text, out var token))
        {
            return;
        }

        viewModel.Calculator.AppendCommand.Execute(token);
        e.Handled = true;
    }
}
