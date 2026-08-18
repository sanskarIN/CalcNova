using CalcNova.App.ViewModels;
using CalcNova.Currency;
using CalcNova.DateTimeTools;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AdvancedUtilityViewModelTests
{
    [Fact]
    public void DateTimeViewModel_CalculatesDifferenceAndBusinessDays()
    {
        var viewModel = new DateTimeViewModel
        {
            StartDate = "2026-08-17",
            EndDate = "2026-08-24"
        };

        viewModel.CalculateDifferenceCommand.Execute(null);

        Assert.Contains("Signed days: 7", viewModel.DifferenceResult, StringComparison.Ordinal);
        Assert.Contains("1 week(s), 0 day(s)", viewModel.DifferenceResult, StringComparison.Ordinal);
        Assert.Contains("Business days (Mon–Fri): 5", viewModel.DifferenceResult, StringComparison.Ordinal);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void DateTimeViewModel_AddsCalendarComponentsInDocumentedOrder()
    {
        var viewModel = new DateTimeViewModel
        {
            StartDate = "2024-02-29",
            Years = "1",
            Months = "1",
            Weeks = "1",
            Days = "2"
        };

        viewModel.AddToDateCommand.Execute(null);

        Assert.Equal("2025-04-09", viewModel.AddResult);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void DateTimeViewModel_ConvertsFixedDurations()
    {
        var viewModel = new DateTimeViewModel
        {
            DurationValue = "2.5",
            DurationFrom = DurationUnit.Hour,
            DurationTo = DurationUnit.Minute
        };

        viewModel.ConvertDurationCommand.Execute(null);

        Assert.Equal("150", viewModel.DurationResult);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void DateTimeViewModel_RejectsAmbiguousDateFormats()
    {
        var viewModel = new DateTimeViewModel
        {
            StartDate = "18/08/2026",
            EndDate = "2026-08-19"
        };

        viewModel.CalculateDifferenceCommand.Execute(null);

        Assert.Empty(viewModel.DifferenceResult);
        Assert.Contains("yyyy-MM-dd", viewModel.ErrorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CurrencyViewModel_UsesLocalCachedSnapshot()
    {
        var cache = new MemoryCache(new CurrencyRateSnapshot(
            "USD",
            new Dictionary<string, decimal> { ["INR"] = 84.25m },
            DateTimeOffset.UtcNow,
            "Test cache"));
        var viewModel = new CurrencyViewModel(cache)
        {
            Amount = "2",
            FromCurrency = "usd",
            ToCurrency = "inr"
        };

        await viewModel.ConvertAsync();

        Assert.Equal("168.50", viewModel.Result);
        Assert.Contains("Test cache", viewModel.RateStatus, StringComparison.Ordinal);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CurrencyViewModel_ExplainsMissingRatesWithoutCrashing()
    {
        var viewModel = new CurrencyViewModel(new MemoryCache(null))
        {
            Amount = "10",
            FromCurrency = "USD",
            ToCurrency = "EUR"
        };

        await viewModel.ConvertAsync();

        Assert.Empty(viewModel.Result);
        Assert.Contains("No currency-rate provider", viewModel.ErrorMessage, StringComparison.Ordinal);
    }

    private sealed class MemoryCache : ICurrencyRateCache
    {
        private CurrencyRateSnapshot? _snapshot;

        public MemoryCache(CurrencyRateSnapshot? snapshot)
        {
            _snapshot = snapshot;
        }

        public Task<CurrencyRateSnapshot?> LoadAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            var normalized = CurrencyCode.Normalize(baseCurrency);
            return Task.FromResult(
                _snapshot is not null && string.Equals(_snapshot.BaseCurrency, normalized, StringComparison.OrdinalIgnoreCase)
                    ? _snapshot
                    : null);
        }

        public Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            _snapshot = snapshot;
            return Task.CompletedTask;
        }
    }
}
