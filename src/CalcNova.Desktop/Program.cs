using Avalonia;
using CalcNova.App;
using CalcNova.App.Services;
using CalcNova.Persistence.History;
using CalcNova.Persistence.Settings;

namespace CalcNova.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppComposition.Configure(CreateDependencies());
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static AppDependencies CreateDependencies()
    {
        var localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localData))
        {
            localData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".calcnova");
        }

        var appDataDirectory = Path.Combine(localData, "CalcNova");
        return new AppDependencies(
            new SqliteCalculationHistoryRepository(Path.Combine(appDataDirectory, "history.db")),
            new JsonSettingsRepository(Path.Combine(appDataDirectory, "settings.json")));
    }
}
