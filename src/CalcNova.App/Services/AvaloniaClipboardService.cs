using Avalonia.Input.Platform;
using CalcNova.Platform.Clipboard;

namespace CalcNova.App.Services;

public sealed class AvaloniaClipboardService : IClipboardService
{
    private IClipboard? _clipboard;

    public bool IsAvailable => _clipboard is not null;

    public void Attach(IClipboard? clipboard) => _clipboard = clipboard;

    public async Task<string?> GetTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var clipboard = _clipboard ?? throw new InvalidOperationException("Clipboard access is unavailable on this platform.");
        var text = await clipboard.TryGetTextAsync();
        cancellationToken.ThrowIfCancellationRequested();
        return text;
    }

    public async Task SetTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);
        cancellationToken.ThrowIfCancellationRequested();
        var clipboard = _clipboard ?? throw new InvalidOperationException("Clipboard access is unavailable on this platform.");
        await clipboard.SetTextAsync(text);
        cancellationToken.ThrowIfCancellationRequested();
    }
}
