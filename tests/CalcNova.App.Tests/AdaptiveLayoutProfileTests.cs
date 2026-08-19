using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AdaptiveLayoutProfileTests
{
    [Theory]
    [InlineData(320)]
    [InlineData(599)]
    public void ForWidth_CompactWidths_ReturnCompactProfile(double width)
    {
        var profile = AdaptiveLayoutProfile.ForWidth(width);

        Assert.True(profile.IsCompact);
        Assert.Equal("compact", profile.StyleClass);
        Assert.Equal(8, profile.ShellMargin);
        Assert.Equal(44, profile.TabMinimumWidth);
        Assert.True(profile.AllowHorizontalModeScrolling);
    }

    [Theory]
    [InlineData(600)]
    [InlineData(979)]
    public void ForWidth_MediumWidths_ReturnMediumProfile(double width)
    {
        var profile = AdaptiveLayoutProfile.ForWidth(width);

        Assert.True(profile.IsMedium);
        Assert.Equal("medium", profile.StyleClass);
        Assert.Equal(12, profile.ShellMargin);
        Assert.Equal(48, profile.TabMinimumWidth);
        Assert.False(profile.AllowHorizontalModeScrolling);
    }

    [Theory]
    [InlineData(980)]
    [InlineData(1440)]
    public void ForWidth_ExpandedWidths_ReturnExpandedProfile(double width)
    {
        var profile = AdaptiveLayoutProfile.ForWidth(width);

        Assert.True(profile.IsExpanded);
        Assert.Equal("expanded", profile.StyleClass);
        Assert.Equal(16, profile.ShellMargin);
        Assert.Equal(56, profile.TabMinimumWidth);
        Assert.False(profile.AllowHorizontalModeScrolling);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void ForWidth_InvalidOrUnmeasuredWidth_UsesSafeCompactProfile(double width)
    {
        var profile = AdaptiveLayoutProfile.ForWidth(width);

        Assert.True(profile.IsCompact);
        Assert.True(profile.AllowHorizontalModeScrolling);
    }
}
