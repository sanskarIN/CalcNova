using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class MainClipboardCompositionTests
{
    [Fact]
    public async Task MainViewModel_SharesClipboardWithCopyEnabledModes()
    {
        var clipboard = new FakeClipboardService();
        var dependencies = AppDependencies.Empty with { ClipboardService = clipboard };
        var viewModel = new MainViewModel(dependencies);

        viewModel.Converter.CopyResultCommand.Execute(null);
        Assert.Equal(viewModel.Converter.Result, await clipboard.WaitForWriteAsync());

        clipboard.Reset();
        viewModel.Programmer.CopyRepresentationCommand.Execute("hexadecimal");
        Assert.Equal(viewModel.Programmer.Hexadecimal, await clipboard.WaitForWriteAsync());

        clipboard.Reset();
        viewModel.CodePoint.CopyCodePointResultCommand.Execute(null);
        Assert.Equal(viewModel.CodePoint.CodePointResult, await clipboard.WaitForWriteAsync());

        clipboard.Reset();
        viewModel.Statistics.CopySummaryCommand.Execute(null);
        Assert.Equal(viewModel.Statistics.Summary, await clipboard.WaitForWriteAsync());

        clipboard.Reset();
        viewModel.Matrices.CopyResultCommand.Execute(null);
        Assert.Equal(viewModel.Matrices.Result, await clipboard.WaitForWriteAsync());

        clipboard.Reset();
        viewModel.Graphing.CopyPreviewCommand.Execute(null);
        var graphText = await clipboard.WaitForWriteAsync();
        Assert.Contains(viewModel.Graphing.Summary, graphText, StringComparison.Ordinal);
    }

    private sealed class FakeClipboardService : IClipboardService
    {
        private TaskCompletionSource<string> _writeSignal = CreateSignal();

        public bool IsAvailable => true;

        public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

        public Task SetTextAsync(string text, CancellationToken cancellationToken = default)
        {
            _writeSignal.TrySetResult(text);
            return Task.CompletedTask;
        }

        public Task<string> WaitForWriteAsync() => _writeSignal.Task.WaitAsync(TimeSpan.FromSeconds(2));

        public void Reset() => _writeSignal = CreateSignal();

        private static TaskCompletionSource<string> CreateSignal() =>
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
