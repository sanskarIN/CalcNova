using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using CalcNova.App;
using CalcNova.App.Services;
using CalcNova.Android.Services;
using CalcNova.Persistence.Currency;
using CalcNova.Persistence.History;
using CalcNova.Persistence.Settings;

namespace CalcNova.Android;

[Activity(
    Label = "CalcNova",
    Theme = "@style/CalcNovaTheme",
    MainLauncher = true,
    Icon = "@mipmap/ic_launcher",
    RoundIcon = "@mipmap/ic_launcher_round",
    ConfigurationChanges = ConfigChanges.Orientation |
                           ConfigChanges.ScreenSize |
                           ConfigChanges.ScreenLayout |
                           ConfigChanges.SmallestScreenSize |
                           ConfigChanges.UiMode |
                           ConfigChanges.Density)]
public sealed class MainActivity : AvaloniaMainActivity<SingleViewApp>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var appDataDirectory = FilesDir?.AbsolutePath
            ?? throw new InvalidOperationException("Android local application storage is unavailable.");

        AppComposition.Configure(new AppDependencies(
            new SqliteCalculationHistoryRepository(Path.Combine(appDataDirectory, "history.db")),
            new JsonSettingsRepository(Path.Combine(appDataDirectory, "settings.json")))
        {
            ExternalLinkService = new AndroidExternalLinkService(this),
            HapticFeedbackService = new AndroidHapticFeedbackService(this),
            CurrencyRateCache = new JsonCurrencyRateCache(Path.Combine(appDataDirectory, "currency"))
        });

        return base.CustomizeAppBuilder(builder).WithInterFont();
    }
}
