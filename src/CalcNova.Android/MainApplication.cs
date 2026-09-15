using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using CalcNova.Android.Services;
using CalcNova.App;
using CalcNova.App.Services;
using CalcNova.Persistence.Currency;
using CalcNova.Persistence.History;
using CalcNova.Persistence.Settings;

namespace CalcNova.Android;

/// <summary>
/// Owns Avalonia startup and dependency composition for the Android head.
/// </summary>
/// <remarks>
/// Composition belongs here rather than in the launcher activity: the Avalonia application
/// is created during <see cref="Application.OnCreate"/>, which Android runs before any
/// activity exists, so anything configured from an activity would arrive too late for
/// startup to see it. Properties such as the label and theme stay in AndroidManifest.xml
/// and are merged into the element this attribute generates.
/// </remarks>
[Application]
public sealed class MainApplication : AvaloniaAndroidApplication<SingleViewApp>
{
    public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        var appDataDirectory = FilesDir?.AbsolutePath
            ?? throw new InvalidOperationException("Android local application storage is unavailable.");

        AppComposition.Configure(new AppDependencies(
            new SqliteCalculationHistoryRepository(Path.Combine(appDataDirectory, "history.db")),
            new JsonSettingsRepository(Path.Combine(appDataDirectory, "settings.json")))
        {
            ExternalLinkService = new AndroidExternalLinkService(this),
            ClipboardService = new AvaloniaClipboardService(),
            CurrencyRateCache = new JsonCurrencyRateCache(Path.Combine(appDataDirectory, "currency"))
        });

        return base.CustomizeAppBuilder(builder).WithInterFont();
    }
}
