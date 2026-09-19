using CalcNova.App.ViewModels;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.App.Tests;

/// <summary>
/// The overlay hiding itself and the choice surviving a restart are different things. The
/// headless tests cover the first with no repository attached, so nothing covered the second:
/// that dismissing onboarding is actually written down and read back on the next launch.
/// </summary>
public sealed class OnboardingPersistenceTests
{
    [Fact]
    public async Task CompletingOnboarding_IsStillCompleteOnTheNextLaunch()
    {
        var storage = new RecordingSettingsRepository();

        var firstLaunch = new SettingsViewModel(storage);
        await firstLaunch.LoadAsync();
        Assert.True(firstLaunch.ShouldShowOnboarding);

        await firstLaunch.CompleteOnboardingAsync();
        Assert.False(firstLaunch.ShouldShowOnboarding);
        Assert.Equal(1, storage.SaveCount);

        var secondLaunch = new SettingsViewModel(storage);
        await secondLaunch.LoadAsync();
        Assert.False(secondLaunch.ShouldShowOnboarding);
    }

    [Fact]
    public async Task SkippingOnboarding_IsStillSkippedOnTheNextLaunch()
    {
        var storage = new RecordingSettingsRepository();

        var firstLaunch = new SettingsViewModel(storage);
        await firstLaunch.LoadAsync();
        await firstLaunch.SkipOnboardingAsync();
        Assert.Equal(1, storage.SaveCount);

        var secondLaunch = new SettingsViewModel(storage);
        await secondLaunch.LoadAsync();
        Assert.False(secondLaunch.ShouldShowOnboarding);
    }

    [Fact]
    public async Task ShowingTheIntroductionAgain_BringsItBackOnTheNextLaunch()
    {
        var storage = new RecordingSettingsRepository();

        var firstLaunch = new SettingsViewModel(storage);
        await firstLaunch.LoadAsync();
        await firstLaunch.CompleteOnboardingAsync();
        await firstLaunch.RestartOnboardingAsync();

        var secondLaunch = new SettingsViewModel(storage);
        await secondLaunch.LoadAsync();
        Assert.True(secondLaunch.ShouldShowOnboarding);
    }

    /// <summary>Keeps one settings record in memory, the way a file on disk would.</summary>
    private sealed class RecordingSettingsRepository : ISettingsRepository
    {
        private AppSettings _settings = new();

        public int SaveCount { get; private set; }

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_settings);

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(settings);
            _settings = settings;
            SaveCount++;
            return Task.CompletedTask;
        }
    }
}
