using System.Text.Json;
using System.Text.Json.Serialization;
using CalcNova.Currency;

namespace CalcNova.Persistence.Currency;

public sealed class JsonCurrencyRateCache : ICurrencyRateCache, IDisposable
{
    // Source-generated metadata keeps the cache trim-safe: the iOS head links in Release, and
    // the reflection overloads cannot tell the trimmer which members of the stored snapshot
    // survive. Built from options so the file stays indented.
    private static readonly StoredCurrencySnapshotJsonContext SerializerContext = new(
        new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = true
        });

    private readonly string _directory;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public JsonCurrencyRateCache(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
    }

    public async Task<CurrencyRateSnapshot?> LoadAsync(string baseCurrency, CancellationToken cancellationToken = default)
    {
        var code = CurrencyCode.Normalize(baseCurrency);
        var path = GetPath(code);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }

            await using var stream = File.OpenRead(path);
            var model = await JsonSerializer.DeserializeAsync(stream, SerializerContext.StoredCurrencySnapshot, cancellationToken);
            if (model is null)
            {
                return null;
            }

            return new CurrencyRateSnapshot(model.BaseCurrency, model.Rates, model.RetrievedAt, model.Source);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            // A structurally valid file can still hold values the snapshot rejects,
            // such as a malformed currency code, a null rate table, or a non-positive
            // rate. Treat an unusable cache entry as a cache miss, exactly as
            // unparsable JSON is treated, so the caller can refresh from the provider.
            return null;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        Directory.CreateDirectory(_directory);
        var path = GetPath(snapshot.BaseCurrency);
        var tempPath = path + ".tmp";
        var model = new StoredCurrencySnapshot(
            snapshot.BaseCurrency,
            snapshot.Rates.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase),
            snapshot.RetrievedAt,
            snapshot.Source);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(stream, model, SerializerContext.StoredCurrencySnapshot, cancellationToken);
            }

            File.Move(tempPath, path, overwrite: true);
        }
        finally
        {
            TryDeleteTemp(tempPath);
            _gate.Release();
        }
    }

    public void Dispose() => _gate.Dispose();

    private string GetPath(string baseCurrency) => Path.Combine(_directory, $"{baseCurrency}.json");

    private static void TryDeleteTemp(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

internal sealed record StoredCurrencySnapshot(
    string BaseCurrency,
    Dictionary<string, decimal> Rates,
    DateTimeOffset RetrievedAt,
    string Source);

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(StoredCurrencySnapshot))]
internal sealed partial class StoredCurrencySnapshotJsonContext : JsonSerializerContext;
