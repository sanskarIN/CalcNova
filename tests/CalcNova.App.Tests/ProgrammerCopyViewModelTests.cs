using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerCopyViewModelTests
{
    [Theory]
    [InlineData("binary", "101010")]
    [InlineData("octal", "52")]
    [InlineData("decimal", "42")]
    [InlineData("hexadecimal", "2A")]
    public async Task CopyRepresentationCommand_CopiesRequestedValue(string representation, string expected)
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new ProgrammerViewModel(clipboard);

        viewModel.CopyRepresentationCommand.Execute(representation);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(expected, clipboard.WrittenText);
    }

    [Fact]
    public async Task CopyRepresentationCommand_CopiesFixedWidthBitPattern()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new ProgrammerViewModel(clipboard)
        {
            WordSize = 8,
            Input = "5",
            InputBase = 10
        };
        viewModel.ConvertCommand.Execute(null);

        viewModel.CopyRepresentationCommand.Execute("bits");
        await clipboard.WaitForWriteAsync();

        Assert.Equal("00000101", clipboard.WrittenText);
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
