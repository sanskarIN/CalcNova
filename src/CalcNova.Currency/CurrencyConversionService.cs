namespace CalcNova.Currency;

public sealed class CurrencyConversionService
{
    private readonly ICurrencyRateProvider? _provider;
    private readonly ICurrencyRateCache _cache;
    private readonly TimeProvider _timeProvider;
    private readonly TimeSpan _staleAfter;

    public CurrencyConversionService(
        ICurrencyRateCache cache,
        ICurrencyRateProvider? provider = null,
        TimeSpan? staleAfter = null,
        TimeProvider? timeProvider = null)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _provider = provider;
        _staleAfter = staleAfter ?? TimeSpan.FromHours(12);
        _timeProvider = timeProvider ?? TimeProvider.System;

        if (_staleAfter <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(staleAfter), "Stale-after duration must be positive.");
        }
    }

    public async Task<CurrencyConversionResult> ConvertAsync(
        decimal amount,
        string fromCurrency,
        string toCurrency,
        bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var from = CurrencyCode.Normalize(fromCurrency);
        var to = CurrencyCode.Normalize(toCurrency);
        var now = _timeProvider.GetUtcNow();

        if (from == to)
        {
            return new CurrencyConversionResult(
                amount,
                from,
                to,
                1m,
                amount,
                now,
                "Local identity conversion",
                false,
                false);
        }

        var cached = await _cache.LoadAsync(from, cancellationToken);
        var snapshot = cached;
        var usedCachedFallback = false;
        var shouldRefresh = forceRefresh || snapshot is null || IsStale(snapshot, now);

        if (shouldRefresh && _provider is not null)
        {
            try
            {
                var refreshed = await _provider.GetLatestAsync(from, cancellationToken);
                if (!string.Equals(refreshed.BaseCurrency, from, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        $"Currency provider returned base '{refreshed.BaseCurrency}' when '{from}' was requested.");
                }

                await _cache.SaveAsync(refreshed, cancellationToken);
                snapshot = refreshed;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch when (cached is not null)
            {
                snapshot = cached;
                usedCachedFallback = true;
            }
        }

        if (snapshot is null)
        {
            throw new InvalidOperationException(
                _provider is null
                    ? "No currency-rate provider is configured and no cached rates are available."
                    : "Currency rates could not be loaded and no cached rates are available.");
        }

        var rate = snapshot.GetRate(to);
        decimal converted;
        try
        {
            converted = checked(amount * rate);
        }
        catch (OverflowException exception)
        {
            throw new OverflowException("Converted currency amount exceeds the supported decimal range.", exception);
        }

        return new CurrencyConversionResult(
            amount,
            from,
            to,
            rate,
            converted,
            snapshot.RetrievedAt,
            snapshot.Source,
            IsStale(snapshot, now),
            usedCachedFallback);
    }

    private bool IsStale(CurrencyRateSnapshot snapshot, DateTimeOffset now) =>
        now - snapshot.RetrievedAt > _staleAfter;
}
