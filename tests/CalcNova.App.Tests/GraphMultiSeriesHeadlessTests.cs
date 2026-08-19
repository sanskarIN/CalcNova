using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphMultiSeriesHeadlessTests
{
    [AvaloniaFact]
    public async Task MultiSeriesPlot_UsesSeriesSurfaceAndTextPatternLegend()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.SelectMode(7);
            Dispatcher.UIThread.RunJobs();

            viewModel.Graphing.MultiExpressionsText = "sin(x)\ncos(x)";
            viewModel.Graphing.PlotMultipleCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            var plot = view.GetVisualDescendants().OfType<GraphPlotControl>().Single();
            Assert.Equal(GraphPlotMode.Multiple, viewModel.Graphing.PlotMode);
            Assert.Same(viewModel.Graphing.MultiSeries, plot.Series);
            Assert.Empty(plot.Segments ?? []);

            var legend = view.GetVisualDescendants()
                .OfType<TextBlock>()
                .Single(textBlock => textBlock.Classes.Contains("graph-series-legend"));
            Assert.True(legend.IsVisible);
            Assert.Contains("f1 [solid] — sin(x)", legend.Text, StringComparison.Ordinal);
            Assert.Contains("f2 [long dash] — cos(x)", legend.Text, StringComparison.Ordinal);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task ReturningToSinglePlot_ClearsMultiSeriesLegend()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.SelectMode(7);
            Dispatcher.UIThread.RunJobs();
            viewModel.Graphing.PlotMultipleCommand.Execute(null);
            viewModel.Graphing.Expression = "x * x";
            viewModel.Graphing.PlotCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            var plot = view.GetVisualDescendants().OfType<GraphPlotControl>().Single();
            Assert.Equal(GraphPlotMode.Single, viewModel.Graphing.PlotMode);
            Assert.Same(viewModel.Graphing.Segments, plot.Segments);
            Assert.Empty(plot.Series ?? []);

            var legend = view.GetVisualDescendants()
                .OfType<TextBlock>()
                .Single(textBlock => textBlock.Classes.Contains("graph-series-legend"));
            Assert.False(legend.IsVisible);
            Assert.Equal(string.Empty, legend.Text);
        }
        finally
        {
            window.Close();
        }
    }
}
