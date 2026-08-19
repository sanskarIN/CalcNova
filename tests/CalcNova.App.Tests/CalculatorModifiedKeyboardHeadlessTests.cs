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

    [AvaloniaFact]
    public async Task ShiftedOperators_DoNotInterceptTextBoxEditing()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            var expressionBox = view.GetVisualDescendants()
                .OfType<TextBox>()
                .First(textBox => ReferenceEquals(textBox.DataContext, viewModel.Calculator));
            Assert.True(expressionBox.Focus());
            viewModel.Calculator.Expression = string.Empty;

            window.KeyPressQwerty(PhysicalKey.Digit8, RawInputModifiers.Shift);

            Assert.DoesNotContain("*", viewModel.Calculator.Expression, StringComparison.Ordinal);
        }
        finally
        {
            window.Close();
        }
    }
}
