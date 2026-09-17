using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace CalcNova.Android;

/// <summary>
/// The launcher activity. Avalonia 12 hosts the application on
/// <see cref="MainApplication"/>, so this type only declares how Android should start and
/// reconfigure the window that the shared shell is rendered into.
/// </summary>
[Activity(
    Label = "@string/app_name",
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
public sealed class MainActivity : AvaloniaMainActivity
{
}
