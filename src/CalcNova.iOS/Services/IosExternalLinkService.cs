using CalcNova.Platform.External;
using Foundation;
using UIKit;

namespace CalcNova.iOS.Services;

public sealed class IosExternalLinkService : IExternalLinkService
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

        using var nativeUrl = new NSUrl(uri.AbsoluteUri);
        if (!await UIApplication.SharedApplication.OpenUrlAsync(nativeUrl, new UIApplicationOpenUrlOptions()))
        {
            throw new InvalidOperationException("iOS could not open the requested external link.");
        }
    }
}
