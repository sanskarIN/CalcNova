using Avalonia;
using Avalonia.iOS;
using CalcNova.App;
using CalcNova.App.Services;
using CalcNova.iOS.Services;
using CalcNova.Persistence.Currency;
using CalcNova.Persistence.History;
using CalcNova.Persistence.Settings;
using Foundation;

namespace CalcNova.iOS;

[Register("AppDelegate")]
public sealed class AppDelegate : AvaloniaAppDelegate<SingleViewApp>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localData))
        {
            localData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        var appDataDirectory = Path.Combine(localData, "CalcNova");
        AppComposition.Configure(new AppDependencies(
            new SqliteCalculationHistoryRepository(Path.Combine(appDataDirectory, "history.db")),
            new JsonSettingsRepository(Path.Combine(appDataDirectory, "settings.json")))
        {
            ExternalLinkService = new IosExternalLinkService(),
            ClipboardService = new AvaloniaClipboardService(),
            CurrencyRateCache = new JsonCurrencyRateCache(Path.Combine(appDataDirectory, "currency"))
        });

        return base.CustomizeAppBuilder(builder).WithInterFont();
    }
}
