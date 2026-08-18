using Android.Content;
using CalcNova.Platform.External;

namespace CalcNova.Android.Services;

public sealed class AndroidExternalLinkService : IExternalLinkService
{
    private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        Uri.UriSchemeHttp,
        Uri.UriSchemeHttps,
        Uri.UriSchemeMailto
    };

    private readonly Context _context;

    public AndroidExternalLinkService(Context context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task OpenAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        cancellationToken.ThrowIfCancellationRequested();

        if (!uri.IsAbsoluteUri || !AllowedSchemes.Contains(uri.Scheme))
        {
            throw new InvalidOperationException("Only absolute HTTP, HTTPS, and mailto links may be opened externally.");
        }

        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(uri.AbsoluteUri));
        intent.AddFlags(ActivityFlags.NewTask);
        _context.StartActivity(intent);
        return Task.CompletedTask;
    }
}
