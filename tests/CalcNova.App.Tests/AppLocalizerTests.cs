using CalcNova.App.Localization;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AppLocalizerTests
{
    [Fact]
    public void EnglishCatalog_CoversEverySemanticKey()
    {
        var localizer = new AppLocalizer();

        foreach (var key in Enum.GetValues<AppStringKey>())
        {
            Assert.False(string.IsNullOrWhiteSpace(localizer[key]));
        }
    }

    [Fact]
    public void DefaultCulture_IsEnglish()
    {
        var localizer = new AppLocalizer();

        Assert.Equal("en", localizer.Culture.Name);
        Assert.Single(localizer.SupportedCultures);
        Assert.Equal("en", localizer.SupportedCultures[0].Name);
    }

    [Fact]
    public void EnglishRegionalCulture_IsAcceptedWithoutChangingStringMeaning()
    {
        var localizer = new AppLocalizer();

        var accepted = localizer.TrySetCulture("en-IN");

        Assert.True(accepted);
        Assert.Equal("en-IN", localizer.Culture.Name);
        Assert.Equal("Calculator", localizer[AppStringKey.ModeCalculator]);
    }

    [Theory]
    [InlineData("hi-IN")]
    [InlineData("fr-FR")]
    [InlineData("not-a-real-culture")]
    [InlineData("")]
    public void UnsupportedOrInvalidCulture_IsRejectedAndPreservesEnglish(string cultureName)
    {
        var localizer = new AppLocalizer();

        var accepted = localizer.TrySetCulture(cultureName);

        Assert.False(accepted);
        Assert.Equal("en", localizer.Culture.Name);
    }

    [Fact]
    public void CultureChanged_FiresOnlyWhenEffectiveCultureChanges()
    {
        var localizer = new AppLocalizer();
        var changed = new List<string>();
        localizer.CultureChanged += culture => changed.Add(culture.Name);

        Assert.True(localizer.TrySetCulture("en-IN"));
        Assert.True(localizer.TrySetCulture("en-IN"));
        Assert.False(localizer.TrySetCulture("de-DE"));

        Assert.Equal(["en-IN"], changed);
    }
}
