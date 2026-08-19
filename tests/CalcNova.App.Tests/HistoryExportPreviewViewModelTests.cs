using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using CalcNova.Platform.History;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class HistoryExportPreviewViewModelTests
{
    [Fact]
    public async Task GenerateExportCommand_BoundsLongPreviewWithoutChangingFullExport()
    {
        var repository = new LargeHistoryRepository();
        var viewModel = new HistoryViewModel(repository)
        {
            SelectedExportFormat = HistoryExportFormat.PlainText
        };
        await viewModel.InitializeAsync();
        var expectedFullExport = new HistoryExportService().Export(viewModel.Entries, HistoryExportFormat.PlainText);

        viewModel.GenerateExportCommand.Execute(null);

        Assert.True(viewModel.IsExportPreviewTruncated);
        Assert.True(viewModel.ExportPreview.Length <= 4_096);
        Assert.True(expectedFullExport.Length > viewModel.ExportPreview.Length);
        Assert.Contains("preview truncated", viewModel.ExportPreview, StringComparison.Ordinal);
        Assert.Contains("copy uses the full export", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopyExportCommand_CopiesFullContentWhenPreviewIsTruncated()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new HistoryViewModel(new LargeHistoryRepository(), clipboardService: clipboard)
        {
            SelectedExportFormat = HistoryExportFormat.Csv
        };
        await viewModel.InitializeAsync();
        viewModel.GenerateExportCommand.Execute(null);
        Assert.True(viewModel.IsExportPreviewTruncated);
        var expectedFullExport = new HistoryExportService().Export(viewModel.Entries, HistoryExportFormat.Csv);

        viewModel.CopyExportCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(expectedFullExport, clipboard.WrittenText);
        Assert.NotEqual(viewModel.ExportPreview, clipboard.WrittenText);
    }

    [Fact]
    public async Task ExportFormatChange_ClearsTruncationStateAndCopyRegeneratesSelectedFormat()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new HistoryViewModel(new LargeHistoryRepository(), clipboardService: clipboard)
        {
            SelectedExportFormat = HistoryExportFormat.PlainText
        };
        await viewModel.InitializeAsync();
        viewModel.GenerateExportCommand.Execute(null);
        Assert.True(viewModel.IsExportPreviewTruncated);

        viewModel.SelectedExportFormat = HistoryExportFormat.Json;

        Assert.Empty(viewModel.ExportPreview);
        Assert.False(viewModel.IsExportPreviewTruncated);

        viewModel.CopyExportCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        var expectedJson = new HistoryExportService().Export(viewModel.Entries, HistoryExportFormat.Json);
        Assert.Equal(expectedJson, clipboard.WrittenText);
    }

    private sealed class LargeHistoryRepository : ICalculationHistoryRepository
    {
        private readonly IReadOnlyList<HistoryEntry> _entries = Enumerable.Range(1, 180)
            .Select(index => new HistoryEntry(
                index,
                $"long_expression_{index}_" + new string('x', 48),
                $"long_result_{index}_" + new string('y', 48),
                new DateTimeOffset(2026, 8, 19, 0, 0, 0, TimeSpan.Zero).AddMinutes(index),
                index % 7 == 0))
            .ToArray();

        public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<HistoryEntry>> GetRecentAsync(
            int limit = 100,
            string? query = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<HistoryEntry>>(_entries.Take(limit).ToArray());

        public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ClearAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default) => Task.CompletedTask;
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
