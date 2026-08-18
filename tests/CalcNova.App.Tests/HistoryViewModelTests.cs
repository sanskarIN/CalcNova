using CalcNova.App.ViewModels;
using CalcNova.Platform.History;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class HistoryViewModelTests
{
    [Fact]
    public async Task ClearCommand_RequiresExplicitConfirmation()
    {
        var repository = new MemoryHistoryRepository();
        var viewModel = new HistoryViewModel(repository);
        await viewModel.InitializeAsync();
        await viewModel.RecordAsync("2 + 2", "4");

        viewModel.ClearCommand.Execute(null);

        Assert.True(viewModel.ClearConfirmationRequired);
        Assert.Single(repository.Entries);

        viewModel.ConfirmClearCommand.Execute(null);
        await WaitUntilAsync(() => !viewModel.ClearConfirmationRequired);

        Assert.Empty(repository.Entries);
        Assert.Empty(viewModel.Entries);
        Assert.Equal("History cleared.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task CancelClear_PreservesHistory()
    {
        var repository = new MemoryHistoryRepository();
        var viewModel = new HistoryViewModel(repository);
        await viewModel.InitializeAsync();
        await viewModel.RecordAsync("3 * 7", "21");

        viewModel.ClearCommand.Execute(null);
        viewModel.CancelClearCommand.Execute(null);

        Assert.False(viewModel.ClearConfirmationRequired);
        Assert.Single(repository.Entries);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 50 && !condition(); attempt++)
        {
            await Task.Delay(10);
        }

        Assert.True(condition());
    }

    private sealed class MemoryHistoryRepository : ICalculationHistoryRepository
    {
        private long _nextId = 1;

        public List<HistoryEntry> Entries { get; } = [];

        public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

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
