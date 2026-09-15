using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views;

public partial class MainView
{
    private BivariateStatisticsPanel? _bivariateStatisticsPanelExtension;
    private TextBlock? _aboutReleaseIdentityExtension;

    protected override void OnDataContextChanged(EventArgs eventArgs)
    {
        base.OnDataContextChanged(eventArgs);

        LayoutUpdated -= HandleBivariateStatisticsLayoutUpdated;
        LayoutUpdated += HandleBivariateStatisticsLayoutUpdated;
        DetachBivariateStatisticsPanel();
        DetachAboutReleaseIdentity();
        EnsureBivariateStatisticsPanel();
        EnsureAboutReleaseIdentity();
    }

    private void HandleBivariateStatisticsLayoutUpdated(object? sender, EventArgs eventArgs)
    {
        EnsureBivariateStatisticsPanel();
        EnsureAboutReleaseIdentity();
    }

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

    private void EnsureAboutReleaseIdentity()
    {
        if (_aboutReleaseIdentityExtension?.Parent is not null || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var aboutPanel = this.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(panel => ReferenceEquals(panel.DataContext, viewModel.About));
        if (aboutPanel is null)
        {
            return;
        }

        var existing = aboutPanel.Children
            .OfType<TextBlock>()
            .FirstOrDefault(block => string.Equals(block.Text, viewModel.About.ReleaseLabel, StringComparison.Ordinal));
        if (existing is not null)
        {
            _aboutReleaseIdentityExtension = existing;
            return;
        }

        _aboutReleaseIdentityExtension = new TextBlock
        {
            Text = viewModel.About.ReleaseLabel,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Opacity = 0.82
        };

        var insertionIndex = Math.Min(2, aboutPanel.Children.Count);
        aboutPanel.Children.Insert(insertionIndex, _aboutReleaseIdentityExtension);
    }

    private void DetachBivariateStatisticsPanel()
    {
        if (_bivariateStatisticsPanelExtension?.Parent is Panel parent)
        {
            parent.Children.Remove(_bivariateStatisticsPanelExtension);
        }

        _bivariateStatisticsPanelExtension = null;
    }

    private void DetachAboutReleaseIdentity()
    {
        if (_aboutReleaseIdentityExtension?.Parent is Panel parent)
        {
            parent.Children.Remove(_aboutReleaseIdentityExtension);
        }

        _aboutReleaseIdentityExtension = null;
    }
}
