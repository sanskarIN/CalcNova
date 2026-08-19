using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views;

public partial class MainView
{
    private BivariateStatisticsPanel? _bivariateStatisticsPanelExtension;

    protected override void OnDataContextChanged(EventArgs eventArgs)
    {
        base.OnDataContextChanged(eventArgs);

        LayoutUpdated -= HandleBivariateStatisticsLayoutUpdated;
        LayoutUpdated += HandleBivariateStatisticsLayoutUpdated;
        DetachBivariateStatisticsPanel();
        EnsureBivariateStatisticsPanel();
    }

    private void HandleBivariateStatisticsLayoutUpdated(object? sender, EventArgs eventArgs) =>
        EnsureBivariateStatisticsPanel();

    private void EnsureBivariateStatisticsPanel()
    {
        if (_bivariateStatisticsPanelExtension?.Parent is not null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var statisticsPanel = this.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(panel => ReferenceEquals(panel.DataContext, viewModel.Statistics));
        if (statisticsPanel is null)
        {
            return;
        }

        var existing = statisticsPanel.Children.OfType<BivariateStatisticsPanel>().FirstOrDefault();
        if (existing is not null)
        {
            _bivariateStatisticsPanelExtension = existing;
            return;
        }

        _bivariateStatisticsPanelExtension = new BivariateStatisticsPanel
        {
            DataContext = viewModel.Statistics,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        statisticsPanel.Children.Add(_bivariateStatisticsPanelExtension);
    }

    private void DetachBivariateStatisticsPanel()
    {
        if (_bivariateStatisticsPanelExtension?.Parent is Panel parent)
        {
            parent.Children.Remove(_bivariateStatisticsPanelExtension);
        }

        _bivariateStatisticsPanelExtension = null;
    }
}
