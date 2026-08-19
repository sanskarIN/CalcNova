using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterPreferenceNoticeHeadlessTests
{
    [AvaloniaFact]
    public async Task ConverterMode_ShowsLocalPreferencePrivacyNotice()
    {
        var (window, view, viewModel) = await CreateReadyWindowAsync();
        try
        {
            viewModel.SelectMode(3);
            Dispatcher.UIThread.RunJobs();

            var notice = view.GetVisualDescendants()
                .OfType<Border>()
                .Single(border => border.Classes.Contains("converter-preference-notice"));
            var text = notice.GetVisualDescendants().OfType<TextBlock>().Select(item => item.Text).ToArray();

            Assert.Contains("Saved converter preferences", text);
            Assert.Contains(
                "Precision, recent pairs, and favorites are stored only in local app settings. Fixed unit conversion remains offline.",
                text);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HindiCulture_LocalizesConverterPreferencePrivacyNotice()
    {
        var (window, view, viewModel) = await CreateReadyWindowAsync();
        try
        {
            viewModel.SelectMode(3);
            Dispatcher.UIThread.RunJobs();
            viewModel.Settings.CultureName = "hi-IN";
            await viewModel.Settings.SaveAsync();
            Dispatcher.UIThread.RunJobs();

            var notice = view.GetVisualDescendants()
                .OfType<Border>()
                .Single(border => border.Classes.Contains("converter-preference-notice"));
            var text = notice.GetVisualDescendants().OfType<TextBlock>().Select(item => item.Text).ToArray();

            Assert.Contains("सहेजी गई कन्वर्टर प्राथमिकताएँ", text);
            Assert.Contains(
                "परिशुद्धता, हाल की जोड़ियाँ और पसंदीदा जोड़ियाँ केवल स्थानीय ऐप सेटिंग्स में संग्रहीत होती हैं। निश्चित इकाई रूपांतरण ऑफ़लाइन रहता है।",
                text);
        }
        finally
        {
            window.Close();
        }
    }

    private static async Task<(Window Window, MainView View, MainViewModel ViewModel)> CreateReadyWindowAsync()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };
        window.Show();
        return (window, view, viewModel);
    }
}
