namespace CalcNova.Currency;

public sealed record CurrencyConversionResult(
    decimal InputAmount,
    string FromCurrency,
    string ToCurrency,
    decimal ExchangeRate,
    decimal ConvertedAmount,
    DateTimeOffset RateTimestamp,
    string Source,
    bool IsStale,
    bool UsedCachedFallback);
