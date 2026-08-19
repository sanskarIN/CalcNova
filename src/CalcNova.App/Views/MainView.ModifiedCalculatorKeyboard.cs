using Avalonia.Controls;
using Avalonia.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views;

public partial class MainView
{
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled ||
            DataContext is not MainViewModel viewModel ||
            viewModel.Settings.ShouldShowOnboarding ||
            viewModel.SelectedModeIndex != 0 ||
            e.Source is TextBox)
        {
            return;
        }

        if (!CalculatorKeyboardInput.TryGetModifiedToken(e.Key, e.KeyModifiers, out var token))
        {
            return;
        }

        viewModel.Calculator.AppendCommand.Execute(token);
        e.Handled = true;
    }
}
