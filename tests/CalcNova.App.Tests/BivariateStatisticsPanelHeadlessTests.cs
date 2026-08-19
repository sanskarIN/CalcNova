using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class BivariateStatisticsPanelHeadlessTests
{
    [AvaloniaFact]
    public void Panel_BindsPairedInputsCommandsAndResults()
    {
        var viewModel = new StatisticsViewModel
        {
            PairedXText = "1,2,3,4",
            PairedYText = "3,5,7,9",
            PredictionX = "5"
        };
        var panel = new BivariateStatisticsPanel { DataContext = viewModel };
        var window = new Window { Width = 720, Height = 640, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var textBoxes = panel.GetVisualDescendants().OfType<TextBox>().ToArray();
            Assert.Equal(3, textBoxes.Length);
            Assert.Contains(textBoxes, box => string.Equals(box.Text, "1,2,3,4", StringComparison.Ordinal));
            Assert.Contains(textBoxes, box => string.Equals(box.Text, "3,5,7,9", StringComparison.Ordinal));
            Assert.Contains(textBoxes, box => string.Equals(box.Text, "5", StringComparison.Ordinal));

            var buttons = panel.GetVisualDescendants().OfType<Button>().ToArray();
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.AnalyzePairsCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.CopyBivariateSummaryCommand));
            Assert.Contains(buttons, button => ReferenceEquals(button.Command, viewModel.PredictCommand));

            viewModel.AnalyzePairsCommand.Execute(null);
            viewModel.PredictCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                panel.GetVisualDescendants().OfType<TextBlock>(),
                block => block.Text?.Contains("Pearson r: 1", StringComparison.Ordinal) == true);
            Assert.Contains(
                panel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "ŷ(5) = 11", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
