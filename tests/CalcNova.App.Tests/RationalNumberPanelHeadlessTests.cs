using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class RationalNumberPanelHeadlessTests
{
    [AvaloniaFact]
    public void Panel_BindsInputsAndExactArithmeticCommands()
    {
        var panel = new RationalNumberPanel();
        var viewModel = Assert.IsType<RationalNumberViewModel>(panel.DataContext);
        var window = new Window { Width = 680, Height = 520, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var textBoxes = panel.GetVisualDescendants().OfType<TextBox>().ToArray();
            Assert.Equal(2, textBoxes.Length);
            Assert.Contains(textBoxes, box => string.Equals(box.Text, "1/3", StringComparison.Ordinal));
            Assert.Contains(textBoxes, box => string.Equals(box.Text, "1/6", StringComparison.Ordinal));

            var buttons = panel.GetVisualDescendants().OfType<Button>().ToArray();
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.NormalizeCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.AddCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.SubtractCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.MultiplyCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.DivideCommand));

            viewModel.LeftText = "0.1";
            viewModel.RightText = "0.2";
            viewModel.AddCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                panel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "1/10 + 1/5 = 3/10", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
