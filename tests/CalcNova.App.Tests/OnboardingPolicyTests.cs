using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class OnboardingPolicyTests
{
    [Theory]
    [InlineData(-10)]
    [InlineData(-1)]
    [InlineData(0)]
    public void ShouldShow_UnseenOrInvalidVersion_ReturnsTrue(int completedVersion)
    {
        Assert.True(OnboardingPolicy.ShouldShow(completedVersion));
    }

    [Fact]
    public void ShouldShow_CurrentVersion_ReturnsFalse()
    {
        Assert.False(OnboardingPolicy.ShouldShow(OnboardingPolicy.CurrentVersion));
    }

    [Fact]
    public void ShouldShow_FutureVersion_ReturnsFalse()
    {
        Assert.False(OnboardingPolicy.ShouldShow(OnboardingPolicy.CurrentVersion + 1));
    }

    [Fact]
    public void MarkCurrentVersionCompleted_ReturnsCurrentVersion()
    {
        Assert.Equal(OnboardingPolicy.CurrentVersion, OnboardingPolicy.MarkCurrentVersionCompleted());
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(3, 3)]
    public void NormalizeCompletedVersion_ClampsNegativeValues(int input, int expected)
    {
        Assert.Equal(expected, OnboardingPolicy.NormalizeCompletedVersion(input));
    }
}
