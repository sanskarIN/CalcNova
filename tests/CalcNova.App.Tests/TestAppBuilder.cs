using Avalonia;
using Avalonia.Headless;
using CalcNova.App;

[assembly: AvaloniaTestApplication(typeof(CalcNova.App.Tests.TestAppBuilder))]

namespace CalcNova.App.Tests;

public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder
        .Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
