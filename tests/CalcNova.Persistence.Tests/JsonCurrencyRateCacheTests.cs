using CalcNova.Currency;
using CalcNova.Persistence.Currency;
using Xunit;

namespace CalcNova.Persistence.Tests;

public sealed class JsonCurrencyRateCacheTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "CalcNova.Tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SaveAndLoad_RoundTripsSnapshot()
    {
        var cache = new JsonCurrencyRateCache(_directory);
        var retrievedAt = new DateTimeOffset(2026, 8, 18, 12, 0, 0, TimeSpan.Zero);
        var snapshot = new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal>
            {
                ["INR"] = 84.25m,
                ["EUR"] = 0.91m
            },
            retrievedAt,
            "Test rates");

        await cache.SaveAsync(snapshot, TestContext.Current.CancellationToken);
        var loaded = await cache.LoadAsync("usd", TestContext.Current.CancellationToken);

        Assert.NotNull(loaded);
        Assert.Equal("USD", loaded.BaseCurrency);
        Assert.Equal(84.25m, loaded.GetRate("INR"));
        Assert.Equal(0.91m, loaded.GetRate("EUR"));
        Assert.Equal(retrievedAt, loaded.RetrievedAt);
        Assert.Equal("Test rates", loaded.Source);
    }

    [Fact]
    public async Task LoadAsync_MissingBaseReturnsNull()
    {
        var cache = new JsonCurrencyRateCache(_directory);

        var result = await cache.LoadAsync("GBP", TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoadAsync_CorruptCacheReturnsNullInsteadOfCrashing()
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(
            Path.Combine(_directory, "USD.json"),
            "{not valid json",
            TestContext.Current.CancellationToken);
        var cache = new JsonCurrencyRateCache(_directory);

        var result = await cache.LoadAsync("USD", TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
