namespace CalcNova.Currency;

public sealed record CurrencyRateSnapshot
{
    public CurrencyRateSnapshot(
        string baseCurrency,
        IReadOnlyDictionary<string, decimal> rates,
        DateTimeOffset retrievedAt,
        string source)
    {
        BaseCurrency = CurrencyCode.Normalize(baseCurrency);
        ArgumentNullException.ThrowIfNull(rates);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        var normalized = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in rates)
        {
            var code = CurrencyCode.Normalize(pair.Key);
            if (pair.Value <= 0m)
            {
                throw new ArgumentOutOfRangeException(nameof(rates), $"Rate for {code} must be positive.");
            }

            normalized[code] = pair.Value;
        }

        normalized[BaseCurrency] = 1m;
        Rates = normalized;
        RetrievedAt = retrievedAt;
        Source = source.Trim();
    }

    public string BaseCurrency { get; }

    public IReadOnlyDictionary<string, decimal> Rates { get; }

    public DateTimeOffset RetrievedAt { get; }

    public string Source { get; }

    public decimal GetRate(string currencyCode)
    {
        var code = CurrencyCode.Normalize(currencyCode);
        if (!Rates.TryGetValue(code, out var rate))
        {
            throw new KeyNotFoundException($"No rate for currency '{code}' is available in this snapshot.");
        }

        return rate;
    }
}
