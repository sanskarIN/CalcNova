using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Converter;

namespace CalcNova.App.ViewModels;

public sealed class ConverterViewModel : ViewModelBase
{
    private const int MinimumPrecision = 3;
    private const int MaximumPrecision = 15;
    private const int MaximumRecentPairs = 8;
    private readonly UnitConverter _converter = new();
    private UnitCategory _selectedCategory = UnitCategory.Length;
    private IReadOnlyList<UnitDefinition> _availableUnits;
    private UnitDefinition _fromUnit;
    private UnitDefinition _toUnit;
    private string _input = "1";
    private int _precision = MaximumPrecision;
    private string _result = string.Empty;
    private string _errorMessage = string.Empty;
    private ConversionPair? _selectedRecentPair;
    private ConversionPair? _selectedFavoritePair;

    public ConverterViewModel()
    {
        _availableUnits = UnitCatalog.ForCategory(_selectedCategory);
        _fromUnit = _availableUnits[0];
        _toUnit = _availableUnits.Count > 1 ? _availableUnits[1] : _availableUnits[0];
        ConvertCommand = new RelayCommand(_ => Convert());
        SwapCommand = new RelayCommand(_ => Swap());
        ToggleFavoriteCommand = new RelayCommand(_ => ToggleFavorite());
        UseRecentCommand = new RelayCommand(_ => UsePair(SelectedRecentPair));
        UseFavoriteCommand = new RelayCommand(_ => UsePair(SelectedFavoritePair));
        Convert();
    }

    public IReadOnlyList<UnitCategory> Categories { get; } = Enum.GetValues<UnitCategory>();

    public IReadOnlyList<int> PrecisionOptions { get; } = Enumerable.Range(MinimumPrecision, MaximumPrecision - MinimumPrecision + 1).ToArray();

    public ObservableCollection<ConversionPair> RecentPairs { get; } = [];

    public ObservableCollection<ConversionPair> FavoritePairs { get; } = [];

    public UnitCategory SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (!SetField(ref _selectedCategory, value))
            {
                return;
            }

            _availableUnits = UnitCatalog.ForCategory(value);
            OnPropertyChanged(nameof(AvailableUnits));
            FromUnit = _availableUnits[0];
            ToUnit = _availableUnits.Count > 1 ? _availableUnits[1] : _availableUnits[0];
            Convert();
        }
    }

    public IReadOnlyList<UnitDefinition> AvailableUnits => _availableUnits;

    public UnitDefinition FromUnit
    {
        get => _fromUnit;
        set
        {
            if (value is not null && SetField(ref _fromUnit, value))
            {
                OnPropertyChanged(nameof(IsCurrentPairFavorite));
                Convert();
            }
        }
    }

    public UnitDefinition ToUnit
    {
        get => _toUnit;
        set
        {
            if (value is not null && SetField(ref _toUnit, value))
            {
                OnPropertyChanged(nameof(IsCurrentPairFavorite));
                Convert();
            }
        }
    }

    public string Input
    {
        get => _input;
        set => SetField(ref _input, value ?? string.Empty);
    }

    public int Precision
    {
        get => _precision;
        set
        {
            if (value is < MinimumPrecision or > MaximumPrecision)
            {
                return;
            }

            if (SetField(ref _precision, value))
            {
                Convert();
            }
        }
    }

    public string Result
    {
        get => _result;
        private set => SetField(ref _result, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ConversionPair? SelectedRecentPair
    {
        get => _selectedRecentPair;
        set => SetField(ref _selectedRecentPair, value);
    }

    public ConversionPair? SelectedFavoritePair
    {
        get => _selectedFavoritePair;
        set => SetField(ref _selectedFavoritePair, value);
    }

    public bool IsCurrentPairFavorite => FindPair(FavoritePairs, FromUnit, ToUnit) is not null;

    public ICommand ConvertCommand { get; }

    public ICommand SwapCommand { get; }

    public ICommand ToggleFavoriteCommand { get; }

    public ICommand UseRecentCommand { get; }

    public ICommand UseFavoriteCommand { get; }

    public void Convert()
    {
        if (!double.TryParse(Input, NumberStyles.Float, CultureInfo.InvariantCulture, out var input) || !double.IsFinite(input))
        {
            Result = string.Empty;
            ErrorMessage = "Enter a finite number using invariant decimal notation.";
            return;
        }

        try
        {
            var converted = _converter.Convert(input, FromUnit.Id, ToUnit.Id);
            Result = $"{converted.ToString($"G{Precision}", CultureInfo.InvariantCulture)} {ToUnit.Symbol}";
            ErrorMessage = string.Empty;
            RecordRecentPair();
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or OverflowException)
        {
            Result = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void Swap()
    {
        (FromUnit, ToUnit) = (ToUnit, FromUnit);
        Convert();
    }

    private void ToggleFavorite()
    {
        var existing = FindPair(FavoritePairs, FromUnit, ToUnit);
        if (existing is not null)
        {
            FavoritePairs.Remove(existing);
            if (Equals(SelectedFavoritePair, existing))
            {
                SelectedFavoritePair = FavoritePairs.FirstOrDefault();
            }
        }
        else
        {
            var pair = new ConversionPair(FromUnit, ToUnit);
            FavoritePairs.Insert(0, pair);
            SelectedFavoritePair = pair;
        }

        OnPropertyChanged(nameof(IsCurrentPairFavorite));
    }

    private void RecordRecentPair()
    {
        var existing = FindPair(RecentPairs, FromUnit, ToUnit);
        if (existing is not null)
        {
            RecentPairs.Remove(existing);
        }

        var pair = new ConversionPair(FromUnit, ToUnit);
        RecentPairs.Insert(0, pair);
        SelectedRecentPair = pair;

        while (RecentPairs.Count > MaximumRecentPairs)
        {
            RecentPairs.RemoveAt(RecentPairs.Count - 1);
        }
    }

    private void UsePair(ConversionPair? pair)
    {
        if (pair is null || pair.From.Category != pair.To.Category)
        {
            return;
        }

        SelectedCategory = pair.From.Category;
        var from = AvailableUnits.FirstOrDefault(unit => unit.Id == pair.From.Id);
        var to = AvailableUnits.FirstOrDefault(unit => unit.Id == pair.To.Id);
        if (from is null || to is null)
        {
            ErrorMessage = "The saved conversion pair is no longer available.";
            return;
        }

        FromUnit = from;
        ToUnit = to;
        Convert();
        ErrorMessage = string.Empty;
    }

    private static ConversionPair? FindPair(
        IEnumerable<ConversionPair> pairs,
        UnitDefinition from,
        UnitDefinition to) =>
        pairs.FirstOrDefault(pair => pair.From.Id == from.Id && pair.To.Id == to.Id);
}

public sealed record ConversionPair(UnitDefinition From, UnitDefinition To)
{
    public override string ToString() => $"{From.Category}: {From.Symbol} → {To.Symbol}";
}
