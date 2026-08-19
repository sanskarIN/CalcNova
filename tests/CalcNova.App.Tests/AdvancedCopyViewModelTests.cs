using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AdvancedCopyViewModelTests
{
    [Fact]
    public async Task StatisticsCopySummaryCommand_CopiesCurrentAnalysis()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new StatisticsViewModel(clipboard)
        {
            DatasetText = "1, 2, 3"
        };
        viewModel.AnalyzeCommand.Execute(null);

        viewModel.CopySummaryCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.Summary, clipboard.WrittenText);
        Assert.Contains("Copied", viewModel.CopyStatus, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task MatrixCopyResultCommand_CopiesCurrentResult()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new MatricesViewModel(clipboard)
        {
            MatrixText = "4, 7\n2, 6"
        };
        viewModel.DeterminantCommand.Execute(null);

        viewModel.CopyResultCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.Result, clipboard.WrittenText);
        Assert.Contains("Copied", viewModel.CopyStatus, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopyCommands_ReportUnavailableClipboard()
    {
        var statistics = new StatisticsViewModel();
        var matrices = new MatricesViewModel();

        statistics.CopySummaryCommand.Execute(null);
        matrices.CopyResultCommand.Execute(null);
        await WaitUntilAsync(() =>
            !string.IsNullOrWhiteSpace(statistics.CopyStatus) &&
            !string.IsNullOrWhiteSpace(matrices.CopyStatus));

        Assert.Contains("not available", statistics.CopyStatus, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not available", matrices.CopyStatus, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task WaitUntilAsync(Func<bool> predicate)
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        while (!predicate())
        {
            await Task.Delay(10, cancellation.Token);
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
