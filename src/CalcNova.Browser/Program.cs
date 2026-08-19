using Avalonia;
using Avalonia.Browser;
using CalcNova.App;
using CalcNova.App.Services;
using CalcNova.Browser.Services;

namespace CalcNova.Browser;

internal static class Program
{
    private static Task Main(string[] args)
    {
        AppComposition.Configure(new AppDependencies(
            new BrowserHistoryRepository(),
            new BrowserSettingsRepository())
        {
            ExternalLinkService = new BrowserExternalLinkService(),
            ClipboardService = new AvaloniaClipboardService(),
            CurrencyRateCache = new BrowserCurrencyRateCache()
        });

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    private static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<SingleViewApp>();
}
