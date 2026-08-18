using System.Diagnostics;
using CalcNova.Platform.External;

namespace CalcNova.Desktop.Services;

public sealed class DesktopExternalLinkService : IExternalLinkService
{
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        Uri.UriSchemeHttp,
        Uri.UriSchemeHttps,
        Uri.UriSchemeMailto
    };

    public Task OpenAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        cancellationToken.ThrowIfCancellationRequested();

        if (!uri.IsAbsoluteUri || !AllowedSchemes.Contains(uri.Scheme))
        {
            throw new InvalidOperationException("Only absolute HTTP, HTTPS, and mailto links may be opened externally.");
        }

        Process.Start(new ProcessStartInfo(uri.AbsoluteUri)
        {
            UseShellExecute = true
        });

        return Task.CompletedTask;
    }
}
