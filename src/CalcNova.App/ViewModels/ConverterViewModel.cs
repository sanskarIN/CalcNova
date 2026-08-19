using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Converter;

namespace CalcNova.App.ViewModels;

public sealed class ConverterViewModel : ViewModelBase
{
    private readonly UnitConverter _converter = new();
    private readonly ConversionPairHistory _pairHistory = new();
    private UnitCategory _selectedCategory = UnitCategory.Length;
    private IReadOnlyList<UnitDefinition> _availableUnits;
    private UnitDefinition _fromUnit;
    private UnitDefinition _toUnit;
    private ConversionPair? _selectedPair;
    private string _input = "1";
    private string _result = string.Empty;
    private string _errorMessage = string.Empty;
    private int _significantDigits = 15;
    private bool _suppressPairRecording;

    public ConverterViewModel()
    {
        _availableUnits = UnitCatalog.ForCategory(_selectedCategory);
        _fromUnit = _availableUnits[0];
        _toUnit = _availableUnits.Count > 1 ? _availableUnits[1] : _availableUnits[0];
        ConvertCommand = new RelayCommand(_ => Convert());
        SwapCommand = new RelayCommand(_ => Swap());
        ToggleFavoriteCommand = new RelayCommand(_ => ToggleCurrentFavorite());
        ApplyPairCommand = new RelayCommand(ApplyPair);
        Convert();
    }

    public IReadOnlyList<UnitCategory> Categories { get; } = Enum.GetValues<UnitCategory>();

    public IReadOnlyList<int> PrecisionOptions { get; } = [6, 9, 12, 15, 17];

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
                Convert();
                NotifyPairStateChanged();
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
                Convert();
                NotifyPairStateChanged();
            }
        }
    }

    public ConversionPair? SelectedPair
    {
        get => _selectedPair;
        set
        {
            if (!SetField(ref _selectedPair, value) || value is null)
            {
                return;
            }

            ApplyPair(value);
        }
    }

    public string Input
    {
        get => _input;
        set => SetField(ref _input, value ?? string.Empty);
    }

    public int SignificantDigits
    {
        get => _significantDigits;
        set
        {
            if (value is < 1 or > 17)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Precision must be between 1 and 17 significant digits.");
            }

            if (SetField(ref _significantDigits, value))
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

    public ConversionPair CurrentPair => new(FromUnit.Id, ToUnit.Id);

    public bool IsCurrentPairFavorite => _pairHistory.IsFavorite(CurrentPair);

    public string FavoriteToggleLabel => IsCurrentPairFavorite ? "Remove favorite" : "Add favorite";

    public IReadOnlyList<ConversionPair> RecentPairs => _pairHistory.Recent;

    public IReadOnlyList<ConversionPair> FavoritePairs => _pairHistory.Favorites;

    public ICommand ConvertCommand { get; }

    public ICommand SwapCommand { get; }

    public ICommand ToggleFavoriteCommand { get; }

    public ICommand ApplyPairCommand { get; }

    private void Convert()
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
            Result = $"{converted.ToString($"G{SignificantDigits}", CultureInfo.InvariantCulture)} {ToUnit.Symbol}";
            ErrorMessage = string.Empty;
            if (!_suppressPairRecording)
            {
                _pairHistory.Record(CurrentPair);
                OnPropertyChanged(nameof(RecentPairs));
            }
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or OverflowException)
        {
            Result = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void Swap()
    {
        _suppressPairRecording = true;
        try
        {
            (FromUnit, ToUnit) = (ToUnit, FromUnit);
        }
        finally
        {
            _suppressPairRecording = false;
        }

        Convert();
    }

    private void ToggleCurrentFavorite()
    {
        _pairHistory.ToggleFavorite(CurrentPair);
        NotifyPairStateChanged();
        OnPropertyChanged(nameof(FavoritePairs));
    }

    private void ApplyPair(object? parameter)
    {
        if (parameter is ConversionPair pair)
        {
            ApplyPair(pair);
        }
    }

    private void ApplyPair(ConversionPair pair)
    {
        _suppressPairRecording = true;
        try
        {
            SelectedCategory = pair.Category;
            FromUnit = UnitCatalog.Get(pair.FromUnitId);
            ToUnit = UnitCatalog.Get(pair.ToUnitId);
        }
        finally
        {
            _suppressPairRecording = false;
        }

        Convert();
    }

    private void NotifyPairStateChanged()
    {
        OnPropertyChanged(nameof(CurrentPair));
        OnPropertyChanged(nameof(IsCurrentPairFavorite));
        OnPropertyChanged(nameof(FavoriteToggleLabel));
    }
}
