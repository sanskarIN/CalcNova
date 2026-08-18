using System.Runtime.InteropServices.JavaScript;

namespace CalcNova.Browser.Services;

internal static partial class BrowserInterop
{
    private static readonly SemaphoreSlim InitializationGate = new(1, 1);
    private static bool _initialized;

    public static async Task EnsureInitializedAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
        {
            return;
        }

        await InitializationGate.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
            {
                return;
            }

            await JSHost.ImportAsync("calcnovaBrowser", "./calcnova-browser.js", cancellationToken);
            _initialized = true;
        }
        finally
        {
            InitializationGate.Release();
        }
    }

    [JSImport("getItem", "calcnovaBrowser")]
    internal static partial string? GetItem(string key);

    [JSImport("setItem", "calcnovaBrowser")]
    internal static partial void SetItem(string key, string value);

    [JSImport("removeItem", "calcnovaBrowser")]
    internal static partial void RemoveItem(string key);

    [JSImport("openExternal", "calcnovaBrowser")]
    internal static partial void OpenExternal(string url);
}
