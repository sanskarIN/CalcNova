using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Core.Evaluation;
using CalcNova.Platform.History;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class StateIntegrationViewModelTests
{
    [Fact]
    public async Task Initialize_LoadsSettingsAndAppliesAngleMode()
    {
        var settingsRepository = new MemorySettingsRepository(new AppSettings
        {
            AngleUnit = AngleUnit.Radians,
            HistoryLimit = 25
        });
        var historyRepository = new MemoryHistoryRepository();
        var viewModel = new MainViewModel(new AppDependencies(historyRepository, settingsRepository));

        await viewModel.InitializeAsync(TestContext.Current.CancellationToken);

        Assert.Equal(AngleUnit.Radians, viewModel.Calculator.AngleUnit);
        Assert.Equal(25, viewModel.Settings.HistoryLimit);
        Assert.True(historyRepository.Initialized);
    }

    [Fact]
    public async Task SuccessfulCalculation_IsRecordedWhenHistoryEnabled()
    {
        var historyRepository = new MemoryHistoryRepository();
        var viewModel = new MainViewModel(new AppDependencies(
            historyRepository,
            new MemorySettingsRepository(new AppSettings { HistoryEnabled = true })));
        await viewModel.InitializeAsync(TestContext.Current.CancellationToken);
        viewModel.Calculator.Expression = "2 + 3";

        await viewModel.Calculator.EvaluateAsync();

        var entry = Assert.Single(historyRepository.Entries);
        Assert.Equal("2 + 3", entry.Expression);
        Assert.Equal("5", entry.Result);
    }

    [Fact]
    public async Task SuccessfulCalculation_IsNotRecordedWhenHistoryDisabled()
    {
        var historyRepository = new MemoryHistoryRepository();
        var viewModel = new MainViewModel(new AppDependencies(
            historyRepository,
            new MemorySettingsRepository(new AppSettings { HistoryEnabled = false })));
        await viewModel.InitializeAsync(TestContext.Current.CancellationToken);
        viewModel.Calculator.Expression = "9 * 9";

        await viewModel.Calculator.EvaluateAsync();

        Assert.Empty(historyRepository.Entries);
    }

    [Fact]
    public async Task SettingsSave_RaisesMainSettingsChangedAndPersists()
    {
        var settingsRepository = new MemorySettingsRepository(new AppSettings());
        var viewModel = new MainViewModel(new AppDependencies(new MemoryHistoryRepository(), settingsRepository));
        await viewModel.InitializeAsync(TestContext.Current.CancellationToken);
        AppSettings? observed = null;
        viewModel.SettingsChanged += settings => observed = settings;
        viewModel.Settings.Theme = ThemePreference.Dark;
        viewModel.Settings.AngleUnit = AngleUnit.Gradians;

        await viewModel.Settings.SaveAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(observed);
        Assert.Equal(ThemePreference.Dark, observed!.Theme);
        Assert.Equal(AngleUnit.Gradians, viewModel.Calculator.AngleUnit);
        Assert.Equal(ThemePreference.Dark, settingsRepository.Settings.Theme);
    }

    private sealed class MemorySettingsRepository : ISettingsRepository
    {
        public MemorySettingsRepository(AppSettings settings)
        {
            Settings = settings;
        }

        public AppSettings Settings { get; private set; }

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default) => Task.FromResult(Settings);

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            Settings = settings;
            return Task.CompletedTask;
        }
    }

    private sealed class MemoryHistoryRepository : ICalculationHistoryRepository
    {
        private long _nextId = 1;

        public bool Initialized { get; private set; }

        public List<HistoryEntry> Entries { get; } = [];

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            Initialized = true;
            return Task.CompletedTask;
        }

        public Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default)
        {
            var entry = new HistoryEntry(_nextId++, expression, result, DateTimeOffset.UtcNow, false);
            Entries.Insert(0, entry);
            return Task.FromResult(entry);
        }

        public Task<IReadOnlyList<HistoryEntry>> GetRecentAsync(int limit = 100, string? query = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<HistoryEntry> entries = Entries;
            if (!string.IsNullOrWhiteSpace(query))
            {
                entries = entries.Where(entry =>
                    entry.Expression.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    entry.Result.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult<IReadOnlyList<HistoryEntry>>(entries.Take(limit).ToArray());
        }

        public Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            Entries.RemoveAll(entry => entry.Id == id);
            return Task.CompletedTask;
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            Entries.Clear();
            return Task.CompletedTask;
        }

        public Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default)
        {
            var index = Entries.FindIndex(entry => entry.Id == id);
            if (index >= 0)
            {
                Entries[index] = Entries[index] with { IsFavorite = isFavorite };
            }

            return Task.CompletedTask;
        }
    }
}
