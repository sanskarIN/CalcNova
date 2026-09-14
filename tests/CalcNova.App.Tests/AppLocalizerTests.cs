using CalcNova.App.Localization;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AppLocalizerTests
{
    [Fact]
    public void EverySupportedCatalog_CoversEverySemanticKey()
    {
        foreach (var culture in new[] { "en", "hi" })
        {
            var localizer = new AppLocalizer(culture);
            foreach (var key in Enum.GetValues<AppStringKey>())
            {
                Assert.False(string.IsNullOrWhiteSpace(localizer[key]));
            }
        }
    }

    [Fact]
    public void DefaultCulture_IsEnglish()
    {
        var localizer = new AppLocalizer();

        Assert.Equal("en", localizer.Culture.Name);
        Assert.Equal(["en", "hi"], localizer.SupportedCultures.Select(culture => culture.Name).ToArray());
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

    [Fact]
    public void HindiRegionalCulture_IsAcceptedAndUsesHindiCatalog()
    {
        var localizer = new AppLocalizer();

        var accepted = localizer.TrySetCulture("hi-IN");

        Assert.True(accepted);
        Assert.Equal("hi-IN", localizer.Culture.Name);
        Assert.Equal("कैलकुलेटर", localizer[AppStringKey.ModeCalculator]);
        Assert.Equal("परिणाम", localizer[AppStringKey.LabelResult]);
    }

    [Theory]
    [InlineData("fr-FR")]
    [InlineData("de-DE")]
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

        Assert.True(localizer.TrySetCulture("hi-IN"));
        Assert.True(localizer.TrySetCulture("hi-IN"));
        Assert.True(localizer.TrySetCulture("en-IN"));
        Assert.False(localizer.TrySetCulture("de-DE"));

        Assert.Equal(["hi-IN", "en-IN"], changed);
    }
}
