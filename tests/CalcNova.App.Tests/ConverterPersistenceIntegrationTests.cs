using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Converter;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterPersistenceIntegrationTests
{
    [Fact]
    public async Task InitializeAsync_RestoresConverterPreferencesFromSettings()
    {
        var recent = ConversionPairToken.Encode(new ConversionPair("km", "m"));
        var favorite = ConversionPairToken.Encode(new ConversionPair("kg", "g"));
        var repository = new FakeSettingsRepository(new AppSettings
        {
            ConverterSignificantDigits = 9,
            ConverterRecentPairs = [recent],
            ConverterFavoritePairs = [favorite]
        });
        var viewModel = new MainViewModel(new AppDependencies(null, repository));

        await viewModel.InitializeAsync();

        Assert.Equal(9, viewModel.Converter.SignificantDigits);
        Assert.Contains(new ConversionPair("km", "m"), viewModel.Converter.RecentPairs);
        Assert.Contains(new ConversionPair("kg", "g"), viewModel.Converter.FavoritePairs);
    }

    [Fact]
    public async Task ConverterPreferenceChange_AutosavesSharedSettings()
    {
        var repository = new FakeSettingsRepository(new AppSettings());
        var viewModel = new MainViewModel(new AppDependencies(null, repository));
        await viewModel.InitializeAsync();
        repository.ExpectNextSave();

        viewModel.Converter.SignificantDigits = 12;

        var saved = await repository.WaitForNextSaveAsync();
        Assert.Equal(12, saved.ConverterSignificantDigits);
    }

    [Fact]
    public async Task FavoritePairChange_AutosavesPairToken()
    {
        var repository = new FakeSettingsRepository(new AppSettings());
        var viewModel = new MainViewModel(new AppDependencies(null, repository));
        await viewModel.InitializeAsync();
        repository.ExpectNextSave();

        viewModel.Converter.ToggleFavoriteCommand.Execute(null);

        var saved = await repository.WaitForNextSaveAsync();
        Assert.Single(saved.ConverterFavoritePairs);
        Assert.True(ConversionPairToken.TryDecode(saved.ConverterFavoritePairs[0], out var pair));
        Assert.Equal(viewModel.Converter.CurrentPair, pair);
    }

    private sealed class FakeSettingsRepository(AppSettings initialSettings) : ISettingsRepository
    {
        private TaskCompletionSource<AppSettings> _nextSave = CreateSignal();

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(initialSettings);

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            _nextSave.TrySetResult(settings);
            return Task.CompletedTask;
        }

        public void ExpectNextSave() => _nextSave = CreateSignal();

        public async Task<AppSettings> WaitForNextSaveAsync() =>
            await _nextSave.Task.WaitAsync(TimeSpan.FromSeconds(2));

        private static TaskCompletionSource<AppSettings> CreateSignal() =>
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
