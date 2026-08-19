using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProductSurfaceLocalizationHeadlessTests
{
    [AvaloniaFact]
    public async Task HindiCulture_LocalizesCurrencySurface()
    {
        var (window, view, viewModel) = await CreateHindiWindowAsync();
        try
        {
            viewModel.SelectMode(9);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "मुद्रा कन्वर्टर");
            Assert.Contains(view.GetVisualDescendants().OfType<TextBox>(), item => Equals(item.Watermark, "राशि"));
            Assert.Contains(view.GetVisualDescendants().OfType<Button>(), item => Equals(item.Content, "दरें रीफ्रेश करें"));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HindiCulture_LocalizesHistorySurface()
    {
        var (window, view, viewModel) = await CreateHindiWindowAsync();
        try
        {
            viewModel.SelectMode(10);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "गणना इतिहास");
            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "दिखाई दे रहा इतिहास निर्यात करें");
            Assert.Contains(view.GetVisualDescendants().OfType<TextBox>(), item => Equals(item.Watermark, "खोजें"));
            Assert.Contains(view.GetVisualDescendants().OfType<Button>(), item => Equals(item.Content, "सभी साफ़ करें"));
            Assert.Contains(view.GetVisualDescendants().OfType<Button>(), item => Equals(item.Content, "निर्यात पूर्वावलोकन बनाएँ"));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HindiCulture_LocalizesSettingsTextActionsAndAccessibilityPreferences()
    {
        var (window, view, viewModel) = await CreateHindiWindowAsync();
        try
        {
            viewModel.SelectMode(11);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "सेटिंग्स");
            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "भाषा");
            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "दशमलव परिशुद्धता (1–29)");
            Assert.Contains(view.GetVisualDescendants().OfType<Button>(), item => Equals(item.Content, "सहेजें"));
            Assert.Contains(view.GetVisualDescendants().OfType<Button>(), item => Equals(item.Content, "रीसेट करें"));
            Assert.Contains(view.GetVisualDescendants().OfType<CheckBox>(), item => Equals(item.Content, "इतिहास सक्षम करें"));
            Assert.Contains(view.GetVisualDescendants().OfType<CheckBox>(), item => Equals(item.Content, "कम गति प्रभाव"));
            Assert.Contains(view.GetVisualDescendants().OfType<CheckBox>(), item => Equals(item.Content, "उच्च कंट्रास्ट प्राथमिकता"));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HindiCulture_LocalizesAboutSurfaceAndPersistentFooter()
    {
        var (window, view, viewModel) = await CreateHindiWindowAsync();
        try
        {
            viewModel.SelectMode(12);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "ओपन-सोर्स कैलकुलेटर • Apache-2.0");
            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "व्यावसायिक संपर्क");
            Assert.Contains(view.GetVisualDescendants().OfType<Button>(), item => Equals(item.Content, "CalcNova रिपॉज़िटरी खोलें"));
            Assert.Contains(view.GetVisualDescendants().OfType<TextBlock>(), item => item.Text == "गणनाएँ डिफ़ॉल्ट रूप से स्थानीय रहती हैं • github.com/sanskarIN/CalcNova");
        }
        finally
        {
            window.Close();
        }
    }

    private static async Task<(Window Window, MainView View, MainViewModel ViewModel)> CreateHindiWindowAsync()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };
        window.Show();

        viewModel.Settings.CultureName = "hi-IN";
        await viewModel.Settings.SaveAsync();
        Dispatcher.UIThread.RunJobs();

        return (window, view, viewModel);
    }
}
