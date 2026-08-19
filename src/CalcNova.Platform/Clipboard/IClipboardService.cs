namespace CalcNova.Platform.Clipboard;

public interface IClipboardService
{
    bool IsAvailable { get; }

    Task<string?> GetTextAsync(CancellationToken cancellationToken = default);

    Task SetTextAsync(string text, CancellationToken cancellationToken = default);
}
