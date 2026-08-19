using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using CalcNova.Platform.History;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class HistoryExportViewModelTests
{
    [Fact]
    public async Task GenerateExportCommand_CreatesSelectedFormatFromVisibleEntries()
    {
        var viewModel = new HistoryViewModel(new FakeHistoryRepository());
        await viewModel.InitializeAsync();
        viewModel.SelectedExportFormat = HistoryExportFormat.Csv;

        viewModel.GenerateExportCommand.Execute(null);

        Assert.StartsWith("id,created_at,expression,result,is_favorite", viewModel.ExportPreview, StringComparison.Ordinal);
        Assert.Contains("1 + 1", viewModel.ExportPreview, StringComparison.Ordinal);
        Assert.Contains("2", viewModel.ExportPreview, StringComparison.Ordinal);
        Assert.Contains("Prepared Csv export for 2 history entries", viewModel.StatusMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SelectedExportFormatChange_ClearsStalePreview()
    {
        var viewModel = new HistoryViewModel(new FakeHistoryRepository());
        await viewModel.InitializeAsync();
        viewModel.GenerateExportCommand.Execute(null);
        Assert.NotEmpty(viewModel.ExportPreview);

        viewModel.SelectedExportFormat = HistoryExportFormat.Json;

        Assert.Empty(viewModel.ExportPreview);
    }

    [Fact]
    public async Task CopyExportCommand_GeneratesAndCopiesWhenPreviewIsEmpty()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new HistoryViewModel(new FakeHistoryRepository(), clipboardService: clipboard)
        {
            SelectedExportFormat = HistoryExportFormat.PlainText
        };
        await viewModel.InitializeAsync();

        viewModel.CopyExportCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.ExportPreview, clipboard.WrittenText);
        Assert.Contains("1 + 1 = 2", clipboard.WrittenText!, StringComparison.Ordinal);
    }

    private sealed class FakeHistoryRepository : ICalculationHistoryRepository
    {
        private readonly List<HistoryEntry> _entries =
        [
            new(1, "1 + 1", "2", new DateTimeOffset(2026, 8, 19, 1, 0, 0, TimeSpan.Zero), true),
            new(2, "sqrt(9)", "3", new DateTimeOffset(2026, 8, 19, 2, 0, 0, TimeSpan.Zero), false)
        ];

        public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default)
        {
            var entry = new HistoryEntry(_entries.Count + 1, expression, result, DateTimeOffset.UtcNow, false);
            _entries.Insert(0, entry);
            return Task.FromResult(entry);
        }

        public Task<IReadOnlyList<HistoryEntry>> GetRecentAsync(int limit = 100, string? query = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<HistoryEntry> result = _entries;
            if (!string.IsNullOrWhiteSpace(query))
            {
                result = result.Where(entry =>
                    entry.Expression.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    entry.Result.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult<IReadOnlyList<HistoryEntry>>(result.Take(limit).ToArray());
        }

        public Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            _entries.RemoveAll(entry => entry.Id == id);
            return Task.CompletedTask;
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            _entries.Clear();
            return Task.CompletedTask;
        }

        public Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default)
        {
            var index = _entries.FindIndex(entry => entry.Id == id);
            if (index >= 0)
            {
                _entries[index] = _entries[index] with { IsFavorite = isFavorite };
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FakeClipboardService : IClipboardService
    {
        private readonly TaskCompletionSource _writeSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool IsAvailable => true;

        public string? WrittenText { get; private set; }

        public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

        public Task SetTextAsync(string text, CancellationToken cancellationToken = default)
        {
            WrittenText = text;
            _writeSignal.TrySetResult();
            return Task.CompletedTask;
        }

        public Task WaitForWriteAsync() => _writeSignal.Task.WaitAsync(TimeSpan.FromSeconds(2));
    }
}
