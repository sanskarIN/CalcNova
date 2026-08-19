using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphCopyViewModelTests
{
    [Fact]
    public async Task CopyPreviewCommand_CopiesSummaryAndPreview()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            Expression = "x",
            MinimumX = "0",
            MaximumX = "1",
            SampleCount = 4
        };
        viewModel.PlotCommand.Execute(null);

        viewModel.CopyPreviewCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.NotNull(clipboard.WrittenText);
        Assert.Contains(viewModel.Summary, clipboard.WrittenText, StringComparison.Ordinal);
        Assert.Contains("0 → 0", clipboard.WrittenText, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CopyAnalysisResultCommand_CopiesApproximateAnalysisText()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            Expression = "x ^ 2",
            AnalysisX = "3"
        };
        viewModel.DerivativeCommand.Execute(null);

        viewModel.CopyAnalysisResultCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.AnalysisResult, clipboard.WrittenText);
        Assert.Contains("≈", clipboard.WrittenText!, StringComparison.Ordinal);
    }

    private sealed class FakeClipboardService : IClipboardService
    {
        private TaskCompletionSource _writeSignal = CreateSignal();

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

        private static TaskCompletionSource CreateSignal() =>
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
