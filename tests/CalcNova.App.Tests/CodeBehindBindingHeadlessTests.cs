using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

/// <summary>
/// Guards the write-back half of the code-behind panel bindings.
/// </summary>
/// <remarks>
/// These panels used reflection bindings until they blocked the trimmed WebAssembly
/// publish, and were rewritten onto compiled bindings that carry explicit accessors. The
/// existing panel tests only prove the view model reaches the controls; these prove edits
/// still travel back the other way, which is the direction the rewrite could have broken
/// silently.
/// </remarks>
public sealed class CodeBehindBindingHeadlessTests
{
    [AvaloniaFact]
    public void EngineeringNotationPanel_WritesEditedTextBackToViewModel()
    {
        var panel = new EngineeringNotationPanel();
        var viewModel = Assert.IsType<EngineeringNotationViewModel>(panel.DataContext);
        var window = new Window { Width = 680, Height = 520, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var input = Assert.Single(
                panel.GetVisualDescendants().OfType<TextBox>(),
                box => box.PlaceholderText == "Finite value or engineering notation");

            input.Text = "8.25e+6";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("8.25e+6", viewModel.InputText);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void EngineeringNotationPanel_WritesEditedPrecisionBackAndIgnoresClearedValue()
    {
        var panel = new EngineeringNotationPanel();
        var viewModel = Assert.IsType<EngineeringNotationViewModel>(panel.DataContext);
        var window = new Window { Width = 680, Height = 520, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var precision = Assert.Single(panel.GetVisualDescendants().OfType<NumericUpDown>());

            precision.Value = 7m;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(7, viewModel.SignificantDigits);

            // An emptied NumericUpDown has no value to convert. Significant digits must keep
            // the last usable setting rather than collapsing to zero, which the formatter
            // rejects outright.
            precision.Value = null;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(7, viewModel.SignificantDigits);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void RationalNumberPanel_WritesBothEditedOperandsBackToViewModel()
    {
        var panel = new RationalNumberPanel();
        var viewModel = Assert.IsType<RationalNumberViewModel>(panel.DataContext);
        var window = new Window { Width = 680, Height = 520, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var textBoxes = panel.GetVisualDescendants().OfType<TextBox>().ToArray();
            var left = Assert.Single(textBoxes, box => box.PlaceholderText == "Left exact value");
            var right = Assert.Single(textBoxes, box => box.PlaceholderText == "Right exact value");

            left.Text = "3/4";
            right.Text = "5/8";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("3/4", viewModel.LeftText);
            Assert.Equal("5/8", viewModel.RightText);

            viewModel.AddCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("3/4 + 5/8 = 11/8", viewModel.OperationSummary);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void BivariateStatisticsPanel_WritesEditedPairedDataBackToInheritedViewModel()
    {
        var viewModel = new StatisticsViewModel();
        var panel = new BivariateStatisticsPanel { DataContext = viewModel };
        var window = new Window { Width = 680, Height = 620, Content = panel };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();

            var textBoxes = panel.GetVisualDescendants().OfType<TextBox>().ToArray();
            var pairedX = Assert.Single(textBoxes, box => box.PlaceholderText == "X values");
            var pairedY = Assert.Single(textBoxes, box => box.PlaceholderText == "Y values");
            var predictionX = Assert.Single(textBoxes, box => box.PlaceholderText == "Prediction X");

            pairedX.Text = "1,2,3,4";
            pairedY.Text = "3,5,7,9";
            predictionX.Text = "5";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("1,2,3,4", viewModel.PairedXText);
            Assert.Equal("3,5,7,9", viewModel.PairedYText);
            Assert.Equal("5", viewModel.PredictionX);

            viewModel.AnalyzePairsCommand.Execute(null);
            viewModel.PredictCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

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
