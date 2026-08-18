using System.Text.Json;
using CalcNova.Currency;

namespace CalcNova.Persistence.Currency;

public sealed class JsonCurrencyRateCache : ICurrencyRateCache
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

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
            var model = await JsonSerializer.DeserializeAsync<StoredSnapshot>(stream, JsonOptions, cancellationToken);
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
        var model = new StoredSnapshot(
            snapshot.BaseCurrency,
            snapshot.Rates.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase),
            snapshot.RetrievedAt,
            snapshot.Source);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(stream, model, JsonOptions, cancellationToken);
            }

            File.Move(tempPath, path, overwrite: true);
        }
        finally
        {
            TryDeleteTemp(tempPath);
            _gate.Release();
        }
    }

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

    private sealed record StoredSnapshot(
        string BaseCurrency,
        Dictionary<string, decimal> Rates,
        DateTimeOffset RetrievedAt,
        string Source);
}
