using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Platform.History;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class FormattingSettingsIntegrationTests
{
    [Fact]
    public async Task Initialize_AppliesPersistedFormattingPreferences()
    {
        var settings = new AppSettings
        {
            DecimalPrecision = 8,
            UseGroupingSeparators = false
        };
        var viewModel = new MainViewModel(new AppDependencies(
            new EmptyHistoryRepository(),
            new MemorySettingsRepository(settings)));

        await viewModel.InitializeAsync(TestContext.Current.CancellationToken);

        Assert.Equal(8, viewModel.Calculator.DecimalPrecision);
        Assert.False(viewModel.Calculator.UseGroupingSeparators);
    }

    [Fact]
    public async Task Save_UpdatesCalculatorFormattingImmediately()
    {
        var repository = new MemorySettingsRepository(new AppSettings());
        var viewModel = new MainViewModel(new AppDependencies(new EmptyHistoryRepository(), repository));
        await viewModel.InitializeAsync(TestContext.Current.CancellationToken);
        viewModel.Settings.DecimalPrecision = 6;
        viewModel.Settings.UseGroupingSeparators = false;

        await viewModel.Settings.SaveAsync(TestContext.Current.CancellationToken);

        Assert.Equal(6, viewModel.Calculator.DecimalPrecision);
        Assert.False(viewModel.Calculator.UseGroupingSeparators);
    }

    private sealed class MemorySettingsRepository : ISettingsRepository
    {
        private AppSettings _settings;

        public MemorySettingsRepository(AppSettings settings)
        {
            _settings = settings;
        }

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default) => Task.FromResult(_settings);

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            _settings = settings;
            return Task.CompletedTask;
        }
    }

    private sealed class EmptyHistoryRepository : ICalculationHistoryRepository
    {
        public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default) =>
            Task.FromResult(new HistoryEntry(1, expression, result, DateTimeOffset.UtcNow, false));

        public Task<IReadOnlyList<HistoryEntry>> GetRecentAsync(int limit = 100, string? query = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<HistoryEntry>>(Array.Empty<HistoryEntry>());

        public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ClearAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
