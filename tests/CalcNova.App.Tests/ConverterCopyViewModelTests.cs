using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterCopyViewModelTests
{
    [Fact]
    public async Task CopyResultCommand_CopiesFormattedConversionResult()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new ConverterViewModel(clipboard)
        {
            Input = "1"
        };
        viewModel.ConvertCommand.Execute(null);

        viewModel.CopyResultCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.Result, clipboard.WrittenText);
        Assert.Contains("copied", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
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
