using CalcNova.Platform.External;

namespace CalcNova.Browser.Services;

public sealed class BrowserExternalLinkService : IExternalLinkService
{
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        Uri.UriSchemeHttp,
        Uri.UriSchemeHttps,
        Uri.UriSchemeMailto
    };

    public async Task OpenAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        cancellationToken.ThrowIfCancellationRequested();

        if (!uri.IsAbsoluteUri || !AllowedSchemes.Contains(uri.Scheme))
        {
            throw new InvalidOperationException("Only absolute HTTP, HTTPS, and mailto links may be opened externally.");
        }

        await BrowserInterop.EnsureInitializedAsync(cancellationToken);
        BrowserInterop.OpenExternal(uri.AbsoluteUri);
    }
}
