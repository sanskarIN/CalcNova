using System.Text.Json;
using CalcNova.Currency;

namespace CalcNova.Browser.Services;

internal sealed class BrowserCurrencyRateCache : ICurrencyRateCache
{
    private const string KeyPrefix = "calcnova.currency.";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<CurrencyRateSnapshot?> LoadAsync(string baseCurrency, CancellationToken cancellationToken = default)
    {
        var code = CurrencyCode.Normalize(baseCurrency);
        await BrowserInterop.EnsureInitializedAsync(cancellationToken);
        var json = BrowserInterop.GetItem(KeyPrefix + code);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var model = JsonSerializer.Deserialize<StoredSnapshot>(json, JsonOptions);
            return model is null
                ? null
                : new CurrencyRateSnapshot(model.BaseCurrency, model.Rates, model.RetrievedAt, model.Source);
        }
        catch (JsonException)
        {
            BrowserInterop.RemoveItem(KeyPrefix + code);
            return null;
        }
    }

    public async Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        await BrowserInterop.EnsureInitializedAsync(cancellationToken);
        var model = new StoredSnapshot(
            snapshot.BaseCurrency,
            snapshot.Rates.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase),
            snapshot.RetrievedAt,
            snapshot.Source);
        BrowserInterop.SetItem(KeyPrefix + snapshot.BaseCurrency, JsonSerializer.Serialize(model, JsonOptions));
    }

    private sealed record StoredSnapshot(
        string BaseCurrency,
        Dictionary<string, decimal> Rates,
        DateTimeOffset RetrievedAt,
        string Source);
}
