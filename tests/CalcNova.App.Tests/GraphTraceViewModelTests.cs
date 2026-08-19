using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphTraceViewModelTests
{
    [Fact]
    public void TraceCommand_ReportsNearestSampledPoint()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2",
            MinimumX = "0",
            MaximumX = "4",
            SampleCount = 5,
            TraceX = "2.2"
        };
        viewModel.PlotCommand.Execute(null);

        viewModel.TraceCommand.Execute(null);

        Assert.Contains("sampled x≈2", viewModel.TraceResult, StringComparison.Ordinal);
        Assert.Contains("y≈4", viewModel.TraceResult, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void TraceCommand_RejectsInvalidTraceX()
    {
        var viewModel = new GraphingViewModel { TraceX = "not-a-number" };

        viewModel.TraceCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.TraceResult);
        Assert.Contains("Trace X", viewModel.ErrorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CopyTraceResultCommand_CopiesTraceText()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            Expression = "x",
            MinimumX = "0",
            MaximumX = "2",
            SampleCount = 3,
            TraceX = "1"
        };
        viewModel.PlotCommand.Execute(null);
        viewModel.TraceCommand.Execute(null);

        viewModel.CopyTraceResultCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.TraceResult, clipboard.WrittenText);
    }

    [Fact]
    public void PlotCommand_ClearsStaleTraceResult()
    {
        var viewModel = new GraphingViewModel { TraceX = "0" };
        viewModel.TraceCommand.Execute(null);
        Assert.NotEqual(string.Empty, viewModel.TraceResult);

        viewModel.Expression = "x";
        viewModel.PlotCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.TraceResult);
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
