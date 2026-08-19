using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class BivariateStatisticsMainViewHeadlessTests
{
    [AvaloniaFact]
    public async Task StatisticsMode_AttachesPairedAnalysisPanelToSharedShell()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            Dispatcher.UIThread.RunJobs();
            var tabs = view.GetVisualDescendants().OfType<TabControl>().First();
            BivariateStatisticsPanel? pairedPanel = null;

            for (var index = 0; index < tabs.ItemCount && pairedPanel is null; index++)
            {
                tabs.SelectedIndex = index;
                Dispatcher.UIThread.RunJobs();
                pairedPanel = view.GetVisualDescendants().OfType<BivariateStatisticsPanel>().FirstOrDefault();
            }

            Assert.NotNull(pairedPanel);
            Assert.Same(viewModel.Statistics, pairedPanel.DataContext);

            viewModel.Statistics.PairedXText = "1,2,3,4";
            viewModel.Statistics.PairedYText = "3,5,7,9";
            viewModel.Statistics.PredictionX = "5";
            viewModel.Statistics.AnalyzePairsCommand.Execute(null);
            viewModel.Statistics.PredictCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                pairedPanel.GetVisualDescendants().OfType<TextBlock>(),
                block => block.Text?.Contains("Regression slope: 2", StringComparison.Ordinal) == true);
            Assert.Contains(
                pairedPanel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "ŷ(5) = 11", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
