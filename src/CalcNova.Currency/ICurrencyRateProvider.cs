namespace CalcNova.Currency;

public interface ICurrencyRateProvider
{
    Task<CurrencyRateSnapshot> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default);
}
