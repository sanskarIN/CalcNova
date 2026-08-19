using CalcNova.App.Infrastructure;
using CalcNova.App.ViewModels;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class SettingsOnboardingViewModelTests
{
    [Fact]
    public void NewViewModel_DoesNotShowOnboardingBeforeSettingsLoad()
    {
        var viewModel = new SettingsViewModel(new RecordingSettingsRepository(new AppSettings()));

        Assert.False(viewModel.IsLoaded);
        Assert.False(viewModel.ShouldShowOnboarding);
    }

    [Fact]
    public async Task LoadAsync_DefaultSettings_RequiresOnboarding()
    {
        var repository = new RecordingSettingsRepository(new AppSettings());
        var viewModel = new SettingsViewModel(repository);

        await viewModel.LoadAsync();

        Assert.True(viewModel.IsLoaded);
        Assert.True(viewModel.ShouldShowOnboarding);
        Assert.Equal(0, viewModel.CompletedOnboardingVersion);
    }

    [Fact]
    public async Task LoadAsync_FailedStorage_DoesNotBlockStartupWithOnboarding()
    {
        var viewModel = new SettingsViewModel(new ThrowingSettingsRepository());

        await viewModel.LoadAsync();

        Assert.False(viewModel.IsLoaded);
        Assert.False(viewModel.ShouldShowOnboarding);
        Assert.Contains("could not be loaded", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CompleteOnboardingAsync_PersistsCurrentVersion()
    {
        var repository = new RecordingSettingsRepository(new AppSettings());
        var viewModel = new SettingsViewModel(repository);
        await viewModel.LoadAsync();

        await viewModel.CompleteOnboardingAsync();

        Assert.False(viewModel.ShouldShowOnboarding);
        Assert.Equal(OnboardingPolicy.CurrentVersion, viewModel.CompletedOnboardingVersion);
        Assert.Equal(OnboardingPolicy.CurrentVersion, repository.Current.CompletedOnboardingVersion);
        Assert.Equal("Onboarding completed.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task SkipOnboardingAsync_PersistsSameCompletionBoundary()
    {
        var repository = new RecordingSettingsRepository(new AppSettings());
        var viewModel = new SettingsViewModel(repository);
        await viewModel.LoadAsync();

        await viewModel.SkipOnboardingAsync();

        Assert.False(viewModel.ShouldShowOnboarding);
        Assert.Equal(OnboardingPolicy.CurrentVersion, repository.Current.CompletedOnboardingVersion);
        Assert.Equal("Onboarding skipped.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task LoadAsync_NegativePersistedVersion_IsNormalizedSafely()
    {
        var repository = new RecordingSettingsRepository(
            new AppSettings { CompletedOnboardingVersion = -50 });
        var viewModel = new SettingsViewModel(repository);

        await viewModel.LoadAsync();

        Assert.Equal(0, viewModel.CompletedOnboardingVersion);
        Assert.True(viewModel.ShouldShowOnboarding);
    }

    private sealed class RecordingSettingsRepository(AppSettings initial) : ISettingsRepository
    {
        public AppSettings Current { get; private set; } = initial;

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Current);
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            Current = settings;
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingSettingsRepository : ISettingsRepository
    {
        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidDataException("Corrupt settings test payload.");
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
