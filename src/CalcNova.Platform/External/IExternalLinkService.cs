namespace CalcNova.Platform.External;

public interface IExternalLinkService
{
    Task OpenAsync(Uri uri, CancellationToken cancellationToken = default);
}
