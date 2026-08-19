using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CodePointCopyViewModelTests
{
    [Fact]
    public async Task CopyCodePointResultCommand_CopiesDecodedResult()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new CodePointViewModel(clipboard)
        {
            CodePointInput = "U+03C0"
        };
        viewModel.DecodeCodePointCommand.Execute(null);

        viewModel.CopyCodePointResultCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.CodePointResult, clipboard.WrittenText);
    }

    [Fact]
    public async Task CopyTextResultCommand_CopiesInspectedScalarList()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new CodePointViewModel(clipboard)
        {
            TextInput = "A😀"
        };
        viewModel.InspectTextCommand.Execute(null);

        viewModel.CopyTextResultCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal("U+0041  U+1F600", clipboard.WrittenText);
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
