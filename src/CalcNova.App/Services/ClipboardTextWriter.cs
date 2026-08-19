using CalcNova.Platform.Clipboard;

namespace CalcNova.App.Services;

public static class ClipboardTextWriter
{
    public static async Task<string> CopyAsync(
        IClipboardService? clipboardService,
        string? text,
        string valueLabel,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueLabel);

        if (clipboardService?.IsAvailable != true)
        {
            return "Clipboard access is unavailable on this platform.";
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return $"There is no {valueLabel} to copy.";
        }

        try
        {
            await clipboardService.SetTextAsync(text, cancellationToken);
            return $"{valueLabel} copied.";
        }
        catch (OperationCanceledException)
        {
            return "Clipboard copy was cancelled.";
        }
        catch (Exception exception) when (exception is InvalidOperationException or NotSupportedException)
        {
            return exception.Message;
        }
    }
}
