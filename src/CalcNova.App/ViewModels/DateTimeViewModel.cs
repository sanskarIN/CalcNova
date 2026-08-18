using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.DateTimeTools;

namespace CalcNova.App.ViewModels;

public sealed class DateTimeViewModel : ViewModelBase
{
    private string _startDate = "2026-01-01";
    private string _endDate = "2026-12-31";
    private string _years = "0";
    private string _months = "0";
    private string _weeks = "0";
    private string _days = "0";
    private string _differenceResult = string.Empty;
    private string _addResult = string.Empty;
    private string _errorMessage = string.Empty;
    private string _durationValue = "1";
    private DurationUnit _durationFrom = DurationUnit.Hour;
    private DurationUnit _durationTo = DurationUnit.Minute;
    private string _durationResult = string.Empty;

    public DateTimeViewModel()
    {
        CalculateDifferenceCommand = new RelayCommand(_ => CalculateDifference());
        AddToDateCommand = new RelayCommand(_ => AddToDate());
        ConvertDurationCommand = new RelayCommand(_ => ConvertDuration());
        SwapDurationUnitsCommand = new RelayCommand(_ => SwapDurationUnits());
        CalculateDifference();
        ConvertDuration();
    }

    public IReadOnlyList<DurationUnit> DurationUnits { get; } = Enum.GetValues<DurationUnit>();

    public string StartDate
    {
        get => _startDate;
        set => SetField(ref _startDate, value ?? string.Empty);
    }

    public string EndDate
    {
        get => _endDate;
        set => SetField(ref _endDate, value ?? string.Empty);
    }

    public string Years
    {
        get => _years;
        set => SetField(ref _years, value ?? string.Empty);
    }

    public string Months
    {
        get => _months;
        set => SetField(ref _months, value ?? string.Empty);
    }

    public string Weeks
    {
        get => _weeks;
        set => SetField(ref _weeks, value ?? string.Empty);
    }

    public string Days
    {
        get => _days;
        set => SetField(ref _days, value ?? string.Empty);
    }

    public string DifferenceResult
    {
        get => _differenceResult;
        private set => SetField(ref _differenceResult, value);
    }

    public string AddResult
    {
        get => _addResult;
        private set => SetField(ref _addResult, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public string DurationValue
    {
        get => _durationValue;
        set => SetField(ref _durationValue, value ?? string.Empty);
    }

    public DurationUnit DurationFrom
    {
        get => _durationFrom;
        set => SetField(ref _durationFrom, value);
    }

    public DurationUnit DurationTo
    {
        get => _durationTo;
        set => SetField(ref _durationTo, value);
    }

    public string DurationResult
    {
        get => _durationResult;
        private set => SetField(ref _durationResult, value);
    }

    public ICommand CalculateDifferenceCommand { get; }

    public ICommand AddToDateCommand { get; }

    public ICommand ConvertDurationCommand { get; }

    public ICommand SwapDurationUnitsCommand { get; }

    private void CalculateDifference()
    {
        try
        {
            var start = ParseDate(StartDate, nameof(StartDate));
            var end = ParseDate(EndDate, nameof(EndDate));
            var difference = DateCalculator.Difference(start, end);
            var businessDays = DateCalculator.BusinessDaysBetween(start, end);
            DifferenceResult = string.Create(
                CultureInfo.InvariantCulture,
                $"Signed days: {difference.SignedDays}\nAbsolute days: {difference.AbsoluteDays}\nWeeks + days: {difference.WholeWeeks} week(s), {difference.RemainingDays} day(s)\nBusiness days (Mon–Fri): {businessDays}");
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentOutOfRangeException or OverflowException)
        {
            DifferenceResult = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void AddToDate()
    {
        try
        {
            var date = ParseDate(StartDate, nameof(StartDate));
            var result = DateCalculator.Add(
                date,
                ParseInt(Years, "Years"),
                ParseInt(Months, "Months"),
                ParseInt(Weeks, "Weeks"),
                ParseInt(Days, "Days"));
            AddResult = result.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentOutOfRangeException or OverflowException)
        {
            AddResult = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void ConvertDuration()
    {
        try
        {
            if (!double.TryParse(DurationValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
            {
                throw new FormatException("Duration value must be a finite number using '.' as the decimal separator.");
            }

            var converted = DurationConverter.Convert(value, DurationFrom, DurationTo);
            DurationResult = converted.ToString("G17", CultureInfo.InvariantCulture);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentOutOfRangeException or OverflowException)
        {
            DurationResult = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void SwapDurationUnits()
    {
        (DurationFrom, DurationTo) = (DurationTo, DurationFrom);
        ConvertDuration();
    }

    private static DateOnly ParseDate(string value, string fieldName)
    {
        if (!DateOnly.TryParseExact(value?.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            throw new FormatException($"{fieldName} must use the yyyy-MM-dd format.");
        }

        return date;
    }

    private static int ParseInt(string value, string fieldName)
    {
        if (!int.TryParse(value?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new FormatException($"{fieldName} must be a whole number.");
        }

        return parsed;
    }
}
