using System.Text.Json;
using System.Text.Json.Serialization;
using CalcNova.Currency;

namespace CalcNova.Browser.Services;

internal sealed class BrowserCurrencyRateCache : ICurrencyRateCache
{
    private const string KeyPrefix = "calcnova.currency.";

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
            var model = JsonSerializer.Deserialize(json, BrowserCurrencyJsonContext.Default.BrowserStoredCurrencySnapshot);
            return model is null
                ? null
                : new CurrencyRateSnapshot(model.BaseCurrency, model.Rates, model.RetrievedAt, model.Source);
        }
        catch (JsonException)
        {
            BrowserInterop.RemoveItem(KeyPrefix + code);
            return null;
        }
        catch (ArgumentException)
        {
            // Stored JSON can parse and still carry values CurrencyRateSnapshot
            // rejects, such as a malformed currency code, a null rate table, or a
            // non-positive rate. Drop the unusable entry and report a cache miss,
            // exactly as unparsable JSON is handled.
            BrowserInterop.RemoveItem(KeyPrefix + code);
            return null;
        }
    }

    public async Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        await BrowserInterop.EnsureInitializedAsync(cancellationToken);
        var model = new BrowserStoredCurrencySnapshot(
            snapshot.BaseCurrency,
            snapshot.Rates.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase),
            snapshot.RetrievedAt,
            snapshot.Source);
        BrowserInterop.SetItem(KeyPrefix + snapshot.BaseCurrency, JsonSerializer.Serialize(model, BrowserCurrencyJsonContext.Default.BrowserStoredCurrencySnapshot));
    }
}

internal sealed record BrowserStoredCurrencySnapshot(
    string BaseCurrency,
    Dictionary<string, decimal> Rates,
    DateTimeOffset RetrievedAt,
    string Source);

// Source-generated metadata keeps this serialization trim-safe: the browser head is
// published with trimming, and the reflection-based overloads cannot tell the trimmer
// which members survive.
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(BrowserStoredCurrencySnapshot))]
internal sealed partial class BrowserCurrencyJsonContext : JsonSerializerContext;
