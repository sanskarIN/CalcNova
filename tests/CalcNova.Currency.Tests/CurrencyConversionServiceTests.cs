using CalcNova.Currency;
using Xunit;

namespace CalcNova.Currency.Tests;

public sealed class CurrencyConversionServiceTests
{
    [Fact]
    public async Task ConvertAsync_UsesFreshProviderSnapshotAndCachesIt()
    {
        var now = new DateTimeOffset(2026, 8, 18, 9, 0, 0, TimeSpan.Zero);
        var provider = new StubProvider(new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["INR"] = 84.25m },
            now,
            "Test provider"));
        var cache = new MemoryCache();
        var service = new CurrencyConversionService(cache, provider, timeProvider: new FixedTimeProvider(now));

        var result = await service.ConvertAsync(2m, "usd", "inr");

        Assert.Equal(168.50m, result.ConvertedAmount);
        Assert.Equal(84.25m, result.ExchangeRate);
        Assert.Equal("USD", result.FromCurrency);
        Assert.Equal("INR", result.ToCurrency);
        Assert.False(result.IsStale);
        Assert.False(result.UsedCachedFallback);
        Assert.NotNull(cache.Snapshot);
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task ConvertAsync_UsesFreshCacheWithoutCallingProvider()
    {
        var now = new DateTimeOffset(2026, 8, 18, 9, 0, 0, TimeSpan.Zero);
        var cache = new MemoryCache
        {
            Snapshot = new CurrencyRateSnapshot(
                "EUR",
                new Dictionary<string, decimal> { ["GBP"] = 0.86m },
                now - TimeSpan.FromMinutes(30),
                "Cached test rates")
        };
        var provider = new StubProvider(new CurrencyRateSnapshot(
            "EUR",
            new Dictionary<string, decimal> { ["GBP"] = 0.90m },
            now,
            "Network test rates"));
        var service = new CurrencyConversionService(
            cache,
            provider,
            staleAfter: TimeSpan.FromHours(12),
            timeProvider: new FixedTimeProvider(now));

        var result = await service.ConvertAsync(100m, "EUR", "GBP");

        Assert.Equal(86m, result.ConvertedAmount);
        Assert.Equal(0, provider.CallCount);
        Assert.False(result.UsedCachedFallback);
        Assert.False(result.IsStale);
    }

    [Fact]
    public async Task ConvertAsync_ProviderFailureFallsBackToStaleCache()
    {
        var now = new DateTimeOffset(2026, 8, 18, 9, 0, 0, TimeSpan.Zero);
        var cache = new MemoryCache
        {
            Snapshot = new CurrencyRateSnapshot(
                "USD",
                new Dictionary<string, decimal> { ["JPY"] = 148m },
                now - TimeSpan.FromDays(1),
                "Yesterday cache")
        };
        var provider = new StubProvider(new IOException("Network unavailable"));
        var service = new CurrencyConversionService(
            cache,
            provider,
            staleAfter: TimeSpan.FromHours(12),
            timeProvider: new FixedTimeProvider(now));

        var result = await service.ConvertAsync(10m, "USD", "JPY");

        Assert.Equal(1480m, result.ConvertedAmount);
        Assert.True(result.IsStale);
        Assert.True(result.UsedCachedFallback);
        Assert.Equal("Yesterday cache", result.Source);
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task ConvertAsync_WithoutProviderOrCacheFailsExplicitly()
    {
        var service = new CurrencyConversionService(new MemoryCache());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ConvertAsync(10m, "USD", "INR"));

        Assert.Contains("No currency-rate provider", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ConvertAsync_SameCurrencyNeverRequiresNetwork()
    {
        var provider = new StubProvider(new IOException("Should not be called"));
        var service = new CurrencyConversionService(new MemoryCache(), provider);

        var result = await service.ConvertAsync(123.45m, "inr", "INR");

        Assert.Equal(123.45m, result.ConvertedAmount);
        Assert.Equal(1m, result.ExchangeRate);
        Assert.Equal(0, provider.CallCount);
        Assert.False(result.IsStale);
    }

    [Fact]
    public async Task ConvertAsync_ForceRefreshUsesProviderEvenWhenCacheIsFresh()
    {
        var now = new DateTimeOffset(2026, 8, 18, 9, 0, 0, TimeSpan.Zero);
        var cache = new MemoryCache
        {
            Snapshot = new CurrencyRateSnapshot(
                "USD",
                new Dictionary<string, decimal> { ["CAD"] = 1.30m },
                now,
                "Cache")
        };
        var provider = new StubProvider(new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["CAD"] = 1.35m },
            now,
            "Provider"));
        var service = new CurrencyConversionService(cache, provider, timeProvider: new FixedTimeProvider(now));

        var result = await service.ConvertAsync(10m, "USD", "CAD", forceRefresh: true);

        Assert.Equal(13.5m, result.ConvertedAmount);
        Assert.Equal("Provider", result.Source);
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task ConvertAsync_ProviderBaseMismatchIsNotSilentlyAcceptedWithoutFallback()
    {
        var provider = new StubProvider(new CurrencyRateSnapshot(
            "EUR",
            new Dictionary<string, decimal> { ["INR"] = 90m },
            DateTimeOffset.UtcNow,
            "Wrong base"));
        var service = new CurrencyConversionService(new MemoryCache(), provider);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            service.ConvertAsync(1m, "USD", "INR"));
    }

    private sealed class MemoryCache : ICurrencyRateCache
    {
        public CurrencyRateSnapshot? Snapshot { get; set; }

        public Task<CurrencyRateSnapshot?> LoadAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            if (Snapshot is null || !string.Equals(Snapshot.BaseCurrency, CurrencyCode.Normalize(baseCurrency), StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<CurrencyRateSnapshot?>(null);
            }

            return Task.FromResult<CurrencyRateSnapshot?>(Snapshot);
        }

        public Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            Snapshot = snapshot;
            return Task.CompletedTask;
        }
    }

    private sealed class StubProvider : ICurrencyRateProvider
    {
        private readonly CurrencyRateSnapshot? _snapshot;
        private readonly Exception? _exception;

        public StubProvider(CurrencyRateSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public StubProvider(Exception exception)
        {
            _exception = exception;
        }

        public int CallCount { get; private set; }

        public Task<CurrencyRateSnapshot> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            CallCount++;
            if (_exception is not null)
            {
                return Task.FromException<CurrencyRateSnapshot>(_exception);
            }

            return Task.FromResult(_snapshot!);
        }
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
