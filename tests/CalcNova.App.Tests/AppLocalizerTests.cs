// tests/CalcNova.App.Tests/AppLocalizerTests.cs
using CalcNova.App.Localization;
using Xunit;

namespace CalcNova.App.Tests;

public class AppLocalizerTests
{
    [Fact]
    public void GetString_WithMissingKey_FallsBackSafelyWithoutThrowing()
    {
        var localizer = AppLocalizer.Instance;
        localizer.CurrentCulture = "hi-IN";

        // Must return localized string or English fallback, never throw KeyNotFoundException
        string text = localizer.GetString(AppStringKey.AppName);
        Assert.False(string.IsNullOrWhiteSpace(text));
    }
}
