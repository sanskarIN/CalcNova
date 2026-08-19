using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorModifiedKeyboardHeadlessTests
{
    [AvaloniaFact]
    public async Task ShiftedTopRowOperators_AppendCanonicalCalculatorTokens()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            var firstTab = view.GetVisualDescendants().OfType<TabItem>().First();
            Assert.True(firstTab.Focus());
            viewModel.Calculator.Clear();

            window.KeyPressQwerty(PhysicalKey.Digit9, RawInputModifiers.Shift);
            window.KeyPressQwerty(PhysicalKey.Digit8, RawInputModifiers.Shift);
            window.KeyPressQwerty(PhysicalKey.Digit6, RawInputModifiers.Shift);
            window.KeyPressQwerty(PhysicalKey.Digit5, RawInputModifiers.Shift);
            window.KeyPressQwerty(PhysicalKey.Digit0, RawInputModifiers.Shift);

            Assert.Equal("(*^%)", viewModel.Calculator.Expression);
        }
        finally
        {
            window.Close();
        }
    }
}
