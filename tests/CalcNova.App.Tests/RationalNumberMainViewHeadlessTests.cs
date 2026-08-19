using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class RationalNumberMainViewHeadlessTests
{
    [AvaloniaFact]
    public async Task CalculatorMode_AttachesExactRationalPanelToSharedShell()
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
            RationalNumberPanel? rationalPanel = null;

            for (var index = 0; index < tabs.ItemCount && rationalPanel is null; index++)
            {
                tabs.SelectedIndex = index;
                Dispatcher.UIThread.RunJobs();
                rationalPanel = view.GetVisualDescendants().OfType<RationalNumberPanel>().FirstOrDefault();
            }

            Assert.NotNull(rationalPanel);
            var rational = Assert.IsType<RationalNumberViewModel>(rationalPanel.DataContext);
            rational.LeftText = "0.1";
            rational.RightText = "0.2";
            rational.AddCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                rationalPanel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "1/10 + 1/5 = 3/10", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
