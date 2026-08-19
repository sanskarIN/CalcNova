using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ModeHeadingLocalizationHeadlessTests
{
    [Theory]
    [InlineData(1, "प्रोग्रामर")]
    [InlineData(3, "कन्वर्टर")]
    [InlineData(4, "सांख्यिकी")]
    [InlineData(5, "समीकरण")]
    [InlineData(6, "मैट्रिक्स")]
    [InlineData(7, "ग्राफ")]
    [InlineData(8, "दिनांक और अवधि")]
    [AvaloniaTheory]
    public async Task HindiCulture_LocalizesPrimaryModeHeading(int modeIndex, string expectedHeading)
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.Settings.CultureName = "hi-IN";
            await viewModel.Settings.SaveAsync();
            viewModel.SelectMode(modeIndex);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                view.GetVisualDescendants().OfType<TextBlock>(),
                textBlock => string.Equals(textBlock.Text, expectedHeading, StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }
}
