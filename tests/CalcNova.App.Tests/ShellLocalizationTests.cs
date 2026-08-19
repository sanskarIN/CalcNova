using CalcNova.App.Localization;
using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ShellLocalizationTests
{
    [Fact]
    public void ModeKeys_CoverEverySharedShellMode()
    {
        Assert.Equal(MainViewModel.ModeCount, ShellLocalization.ModeKeys.Count);
        Assert.Equal(AppStringKey.ModeCalculator, ShellLocalization.ModeKeys[0]);
        Assert.Equal(AppStringKey.ModeAbout, ShellLocalization.ModeKeys[^1]);
        Assert.Equal(ShellLocalization.ModeKeys.Count, ShellLocalization.ModeKeys.Distinct().Count());
    }

    [Fact]
    public void HindiModeHeaders_UseSemanticCatalog()
    {
        var localizer = new AppLocalizer("hi-IN");

        var headers = ShellLocalization.GetModeHeaders(localizer);

        Assert.Equal("कैलकुलेटर", headers[0]);
        Assert.Equal("इतिहास", headers[10]);
        Assert.Equal("परिचय", headers[^1]);
    }

    [Fact]
    public void HindiOnboardingStrings_UseSemanticCatalog()
    {
        var localizer = new AppLocalizer("hi-IN");

        Assert.Equal("CalcNova में आपका स्वागत है", localizer[AppStringKey.OnboardingWelcome]);
        Assert.Equal("अपने तरीके से गणना करें", localizer[AppStringKey.OnboardingCalculateTitle]);
        Assert.Equal("छोड़ें", localizer[AppStringKey.ActionSkip]);
        Assert.Equal("गणना शुरू करें", localizer[AppStringKey.ActionStartCalculating]);
    }

    [Theory]
    [InlineData("CalcNova", AppStringKey.AppName)]
    [InlineData("Paste expression", AppStringKey.ActionPasteExpression)]
    [InlineData("Copy result", AppStringKey.ActionCopyResult)]
    [InlineData("Word size", AppStringKey.LabelWordSize)]
    [InlineData("History limit", AppStringKey.LabelHistoryLimit)]
    [InlineData("Welcome to CalcNova", AppStringKey.OnboardingWelcome)]
    [InlineData("Calculate your way", AppStringKey.OnboardingCalculateTitle)]
    [InlineData("Skip", AppStringKey.ActionSkip)]
    [InlineData("Start calculating", AppStringKey.ActionStartCalculating)]
    public void KnownSharedLiterals_MapToSemanticKeys(string literal, AppStringKey expected)
    {
        Assert.True(ShellLocalization.TryGetLiteralKey(literal, out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void UnknownLiteral_IsNotLocalizedAccidentally()
    {
        Assert.False(ShellLocalization.TryGetLiteralKey("dynamic calculation output", out _));
    }
}
