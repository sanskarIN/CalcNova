using Xunit;

namespace CalcNova.Currency.Tests;

/// <summary>
/// Covers the value types the conversion service is built on.
/// </summary>
/// <remarks>
/// <see cref="CurrencyConversionServiceTests"/> exercises the refresh and fallback paths
/// but takes normalization and snapshot construction on trust. Those are what decide
/// whether a rate table loaded from disk or from a provider is usable at all, so they are
/// pinned here directly.
/// </remarks>
public sealed class CurrencyRateSnapshotTests
{
    [Theory]
    [InlineData("usd", "USD")]
    [InlineData("USD", "USD")]
    [InlineData("  eUr  ", "EUR")]
    [InlineData("\tinr\n", "INR")]
    public void Normalize_UppercasesAndTrims(string input, string expected) =>
        Assert.Equal(expected, CurrencyCode.Normalize(input));

    [Fact]
    public void Normalize_IsIdempotent()
    {
        foreach (var code in new[] { "usd", " eur ", "JPY" })
        {
            var once = CurrencyCode.Normalize(code);

            Assert.Equal(once, CurrencyCode.Normalize(once));
        }
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDD")]
    [InlineData("US1")]
    [InlineData("US$")]
    [InlineData("u s d")]
    public void Normalize_RejectsAnythingThatIsNotThreeAsciiLetters(string input) =>
        Assert.Throws<ArgumentException>(() => CurrencyCode.Normalize(input));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_RejectsBlankInput(string input) =>
        Assert.Throws<ArgumentException>(() => CurrencyCode.Normalize(input));

    [Fact]
    public void Normalize_RejectsNullWithTheDedicatedNullException() =>
        Assert.Throws<ArgumentNullException>(() => CurrencyCode.Normalize(null!));

    [Fact]
    public void Snapshot_AlwaysReportsTheBaseCurrencyAtParity()
    {
        var snapshot = new CurrencyRateSnapshot(
            "usd",
            new Dictionary<string, decimal> { ["USD"] = 42m, ["eur"] = 0.9m },
            DateTimeOffset.UnixEpoch,
            "test");

        Assert.Equal("USD", snapshot.BaseCurrency);
        Assert.Equal(1m, snapshot.GetRate("USD"));
        Assert.Equal(1m, snapshot.GetRate("usd"));
    }

    [Fact]
    public void Snapshot_LooksUpRatesWithoutRegardToCase()
    {
        var snapshot = new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["eur"] = 0.9m },
            DateTimeOffset.UnixEpoch,
            "test");

        Assert.Equal(0.9m, snapshot.GetRate("EUR"));
        Assert.Equal(0.9m, snapshot.GetRate("eur"));
        Assert.Equal(0.9m, snapshot.GetRate(" EuR "));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Snapshot_RejectsNonPositiveRates(int rate) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["EUR"] = rate },
            DateTimeOffset.UnixEpoch,
            "test"));

    [Fact]
    public void Snapshot_RejectsAMissingSource() =>
        Assert.Throws<ArgumentException>(() => new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["EUR"] = 0.9m },
            DateTimeOffset.UnixEpoch,
            "   "));

    [Fact]
    public void Snapshot_ReportsAnUnknownCurrencyRatherThanGuessing()
    {
        var snapshot = new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["EUR"] = 0.9m },
            DateTimeOffset.UnixEpoch,
            "test");

        Assert.Throws<KeyNotFoundException>(() => snapshot.GetRate("JPY"));
    }
}
