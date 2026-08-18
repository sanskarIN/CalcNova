using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Currency;

namespace CalcNova.App.ViewModels;

public sealed class CurrencyViewModel : ViewModelBase
{
    private readonly CurrencyConversionService _service;
    private string _amount = "1";
    private string _fromCurrency = "USD";
    private string _toCurrency = "INR";
    private string _result = string.Empty;
    private string _rateStatus = "No rate loaded yet.";
    private string _errorMessage = string.Empty;

    public CurrencyViewModel(ICurrencyRateCache? cache = null, ICurrencyRateProvider? provider = null)
    {
        _service = new CurrencyConversionService(cache ?? new EmptyCurrencyRateCache(), provider);
        ConvertCommand = new AsyncRelayCommand(_ => ConvertAsync(forceRefresh: false));
        RefreshCommand = new AsyncRelayCommand(_ => ConvertAsync(forceRefresh: true));
    }

    public string Amount
    {
        get => _amount;
        set => SetField(ref _amount, value ?? string.Empty);
    }

    public string FromCurrency
    {
        get => _fromCurrency;
        set => SetField(ref _fromCurrency, value?.ToUpperInvariant() ?? string.Empty);
    }

    public string ToCurrency
    {
        get => _toCurrency;
        set => SetField(ref _toCurrency, value?.ToUpperInvariant() ?? string.Empty);
    }

    public string Result
    {
        get => _result;
        private set => SetField(ref _result, value);
    }

    public string RateStatus
    {
        get => _rateStatus;
        private set => SetField(ref _rateStatus, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand ConvertCommand { get; }

    public ICommand RefreshCommand { get; }

    private async Task ConvertAsync(bool forceRefresh)
    {
        try
        {
            if (!decimal.TryParse(Amount, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            {
                throw new FormatException("Amount must be a decimal number using '.' as the decimal separator.");
            }

            var conversion = await _service.ConvertAsync(amount, FromCurrency, ToCurrency, forceRefresh);
            Result = conversion.ConvertedAmount.ToString("G29", CultureInfo.InvariantCulture);
            var freshness = conversion.IsStale ? "stale cached rate" : "current cached/provided rate";
            var fallback = conversion.UsedCachedFallback ? "; provider unavailable, cached fallback used" : string.Empty;
            RateStatus = $"1 {conversion.FromCurrency} = {conversion.ExchangeRate.ToString("G29", CultureInfo.InvariantCulture)} {conversion.ToCurrency}\n" +
                         $"Rate time: {conversion.RateTimestamp:O}\nSource: {conversion.Source}\nStatus: {freshness}{fallback}";
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or InvalidDataException or KeyNotFoundException or OverflowException)
        {
            Result = string.Empty;
            RateStatus = "No usable rate available.";
            ErrorMessage = exception.Message;
        }
    }

    private sealed class EmptyCurrencyRateCache : ICurrencyRateCache
    {
        public Task<CurrencyRateSnapshot?> LoadAsync(string baseCurrency, CancellationToken cancellationToken = default) =>
            Task.FromResult<CurrencyRateSnapshot?>(null);

        public Task SaveAsync(CurrencyRateSnapshot snapshot, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
