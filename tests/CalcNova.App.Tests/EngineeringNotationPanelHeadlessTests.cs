using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class EngineeringNotationPanelHeadlessTests
{
    [AvaloniaFact]
    public void Panel_FormatsAndParsesThroughBoundCommands()
    {
        var panel = new EngineeringNotationPanel();
        var viewModel = Assert.IsType<EngineeringNotationViewModel>(panel.DataContext);
        var window = new Window { Width = 680, Height = 520, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var buttons = panel.GetVisualDescendants().OfType<Button>().ToArray();
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.FormatCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.ParseCommand));

            viewModel.InputText = "1234567";
            viewModel.SignificantDigits = 6;
            viewModel.FormatCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                panel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "Engineering: 1.23457e+6", StringComparison.Ordinal));

            viewModel.InputText = "12.5e+3";
            viewModel.ParseCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                panel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "Value: 12500", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Panel_ConstrainsPrecisionControlToFormatterContract()
    {
        var panel = new EngineeringNotationPanel();
        var window = new Window { Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();
            var precision = Assert.Single(panel.GetVisualDescendants().OfType<NumericUpDown>());

            Assert.Equal(1m, precision.Minimum);
            Assert.Equal(15m, precision.Maximum);
            Assert.Equal(1m, precision.Increment);
        }
        finally
        {
            window.Close();
        }
    }
}
