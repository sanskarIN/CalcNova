using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class EngineeringNotationMainViewHeadlessTests
{
    [AvaloniaFact]
    public async Task CalculatorMode_AttachesEngineeringNotationPanelToSharedShell()
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
            EngineeringNotationPanel? engineeringPanel = null;

            for (var index = 0; index < tabs.ItemCount && engineeringPanel is null; index++)
            {
                tabs.SelectedIndex = index;
                Dispatcher.UIThread.RunJobs();
                engineeringPanel = view.GetVisualDescendants().OfType<EngineeringNotationPanel>().FirstOrDefault();
            }

            Assert.NotNull(engineeringPanel);
            var engineering = Assert.IsType<EngineeringNotationViewModel>(engineeringPanel.DataContext);
            engineering.InputText = "1234567";
            engineering.SignificantDigits = 6;
            engineering.FormatCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                engineeringPanel.GetVisualDescendants().OfType<TextBlock>(),
                block => string.Equals(block.Text, "Engineering: 1.23457e+6", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
