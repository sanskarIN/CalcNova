using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AboutReleaseIdentityHeadlessTests
{
    [AvaloniaFact]
    public async Task AboutMode_ShowsCompletedReleaseIdentity()
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
            tabs.SelectedIndex = tabs.ItemCount - 1;
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                view.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "Version 2.9.7 • Complete", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
