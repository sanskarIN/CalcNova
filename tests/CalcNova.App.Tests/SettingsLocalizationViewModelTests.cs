using CalcNova.App.Localization;
using CalcNova.App.ViewModels;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class SettingsLocalizationViewModelTests
{
    [Fact]
    public void Constructor_ExposesReviewedSupportedCultures()
    {
        var viewModel = new SettingsViewModel(null, new AppLocalizer());

        Assert.Equal(["en"], viewModel.SupportedCultureNames);
        Assert.Equal("en", viewModel.CultureName);
    }

    [Fact]
    public async Task LoadAsync_EnglishRegionalPreference_UpdatesSharedLocalizer()
    {
        var localizer = new AppLocalizer();
        var repository = new RecordingSettingsRepository(new AppSettings { CultureName = "en-IN" });
        var viewModel = new SettingsViewModel(repository, localizer);

        await viewModel.LoadAsync();

        Assert.Equal("en-IN", viewModel.CultureName);
        Assert.Equal("en-IN", localizer.Culture.Name);
    }

    [Fact]
    public async Task LoadAsync_UnsupportedPreference_FallsBackToEnglish()
    {
        var localizer = new AppLocalizer();
        var repository = new RecordingSettingsRepository(new AppSettings { CultureName = "fr-FR" });
        var viewModel = new SettingsViewModel(repository, localizer);
        AppSettings? applied = null;
        viewModel.SettingsChanged += settings => applied = settings;

        await viewModel.LoadAsync();

        Assert.Equal("en", viewModel.CultureName);
        Assert.Equal("en", localizer.Culture.Name);
        Assert.NotNull(applied);
        Assert.Equal("en", applied!.CultureName);
    }

    [Fact]
    public async Task SaveAsync_EnglishRegionalPreference_IsPersistedNormalized()
    {
        var localizer = new AppLocalizer();
        var repository = new RecordingSettingsRepository(new AppSettings());
        var viewModel = new SettingsViewModel(repository, localizer)
        {
            CultureName = "en-IN"
        };

        await viewModel.SaveAsync();

        Assert.Equal("en-IN", repository.Current.CultureName);
        Assert.Equal("en-IN", localizer.Culture.Name);
        Assert.Equal("Settings saved.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task SaveAsync_UnsupportedPreference_IsRejectedWithoutPersisting()
    {
        var localizer = new AppLocalizer();
        var repository = new RecordingSettingsRepository(new AppSettings());
        var viewModel = new SettingsViewModel(repository, localizer)
        {
            CultureName = "fr-FR"
        };

        await viewModel.SaveAsync();

        Assert.Equal("en", repository.Current.CultureName);
        Assert.Contains("not supported", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);
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
}
