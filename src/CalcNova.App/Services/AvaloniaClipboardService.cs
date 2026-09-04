// CalcNova.App/Services/AvaloniaClipboardService.cs
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CalcNova.Platform.Clipboard;

namespace CalcNova.App.Services;

public sealed class AvaloniaClipboardService : IClipboardService
{
    private static IClipboard? ResolveClipboard()
    {
        var app = Application.Current;
        if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.Clipboard;
        }

        if (app?.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            return TopLevel.GetTopLevel(singleView.MainView)?.Clipboard;
        }

        return null;
    }

    public async Task<bool> SetTextAsync(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        try
        {
            var clipboard = ResolveClipboard();
            if (clipboard == null)
                return false;

            await clipboard.SetTextAsync(text);
            return true;
        }
        catch (Exception)
        {
            // Avoid terminating UI process if OS clipboard service is unavailable
            return false;
        }
    }

    public async Task<string?> GetTextAsync()
    {
        try
        {
            var clipboard = ResolveClipboard();
            return clipboard != null ? await clipboard.GetTextAsync() : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
