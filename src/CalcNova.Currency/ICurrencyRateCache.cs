namespace CalcNova.Currency;

public interface ICurrencyRateCache
{
    Task<CurrencyRateSnapshot?> LoadAsync(string baseCurrency, CancellationToken cancellationToken = default);

    Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default);
}
