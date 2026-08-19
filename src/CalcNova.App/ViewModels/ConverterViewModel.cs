using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.Services;
using CalcNova.Converter;
using CalcNova.Platform.Clipboard;

namespace CalcNova.App.ViewModels;

public sealed class ConverterViewModel : ViewModelBase
{
    private readonly UnitConverter _converter = new();
    private readonly ConversionPairHistory _pairHistory = new();
    private readonly IClipboardService? _clipboardService;
    private UnitCategory _selectedCategory = UnitCategory.Length;
    private IReadOnlyList<UnitDefinition> _availableUnits;
    private UnitDefinition _fromUnit;
    private UnitDefinition _toUnit;
    private ConversionPair? _selectedPair;
    private UnitDefinition? _selectedSearchUnit;
    private string _unitSearchQuery = string.Empty;
    private IReadOnlyList<UnitDefinition> _searchResults = Array.Empty<UnitDefinition>();
    private string _input = "1";
    private string _result = string.Empty;
    private string _errorMessage = string.Empty;
    private int _significantDigits = 15;
    private bool _suppressPairRecording;
    private bool _suppressPersistenceNotifications;

    public ConverterViewModel(IClipboardService? clipboardService = null)
    {
        _clipboardService = clipboardService;
        _availableUnits = UnitCatalog.ForCategory(_selectedCategory);
        _fromUnit = _availableUnits[0];
        _toUnit = _availableUnits.Count > 1 ? _availableUnits[1] : _availableUnits[0];
        ConvertCommand = new RelayCommand(_ => Convert(recordPair: true));
        SwapCommand = new RelayCommand(_ => Swap());
        ToggleFavoriteCommand = new RelayCommand(_ => ToggleCurrentFavorite());
        ApplyPairCommand = new RelayCommand(ApplyPair);
        UseSearchAsFromCommand = new RelayCommand(_ => UseSearchUnit(asFrom: true));
        UseSearchAsToCommand = new RelayCommand(_ => UseSearchUnit(asFrom: false));
        ClearRecentCommand = new RelayCommand(_ => ClearRecent());
        CopyResultCommand = new AsyncRelayCommand(_ => CopyResultAsync());
        RefreshSearchResults();
        Convert();
    }

    public event Action? PersistenceStateChanged;

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
            SelectedSearchUnit = null;
            RefreshSearchResults();
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
            if (value is null)
            {
                SetField(ref _selectedPair, null);
                return;
            }

            _selectedPair = value;
            OnPropertyChanged();
            ApplyPair(value);
            _selectedPair = null;
            OnPropertyChanged();
        }
    }

    public string UnitSearchQuery
    {
        get => _unitSearchQuery;
        set
        {
            if (SetField(ref _unitSearchQuery, value ?? string.Empty))
            {
                SelectedSearchUnit = null;
                RefreshSearchResults();
            }
        }
    }

    public IReadOnlyList<UnitDefinition> SearchResults
    {
        get => _searchResults;
        private set => SetField(ref _searchResults, value);
    }

    public UnitDefinition? SelectedSearchUnit
    {
        get => _selectedSearchUnit;
        set => SetField(ref _selectedSearchUnit, value);
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
                NotifyPersistenceStateChanged();
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

    public ICommand UseSearchAsFromCommand { get; }

    public ICommand UseSearchAsToCommand { get; }

    public ICommand ClearRecentCommand { get; }

    public ICommand CopyResultCommand { get; }

    public string[] GetRecentPairTokens() => RecentPairs.Select(ConversionPairToken.Encode).ToArray();

    public string[] GetFavoritePairTokens() => FavoritePairs.Take(100).Select(ConversionPairToken.Encode).ToArray();

    public void RestorePersistedState(IEnumerable<string>? recentTokens, IEnumerable<string>? favoriteTokens, int significantDigits)
    {
        var recentPairs = DecodeTokens(recentTokens);
        var favoritePairs = DecodeTokens(favoriteTokens).Take(100).ToArray();

        _suppressPersistenceNotifications = true;
        try
        {
            _pairHistory.Restore(recentPairs, favoritePairs);
            _significantDigits = significantDigits is >= 1 and <= 17 ? significantDigits : 15;
            OnPropertyChanged(nameof(SignificantDigits));
            OnPropertyChanged(nameof(RecentPairs));
            OnPropertyChanged(nameof(FavoritePairs));
            NotifyPairStateChanged();
            Convert();
        }
        finally
        {
            _suppressPersistenceNotifications = false;
        }
    }

    private void Convert(bool recordPair = false)
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
            if (recordPair && !_suppressPairRecording && _pairHistory.Record(CurrentPair))
            {
                OnPropertyChanged(nameof(RecentPairs));
                NotifyPersistenceStateChanged();
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

        Convert(recordPair: true);
    }

    private void ToggleCurrentFavorite()
    {
        _pairHistory.ToggleFavorite(CurrentPair);
        NotifyPairStateChanged();
        OnPropertyChanged(nameof(FavoritePairs));
        NotifyPersistenceStateChanged();
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

        Convert(recordPair: true);
    }

    private void RefreshSearchResults()
    {
        SearchResults = UnitSearch.Search(SelectedCategory, UnitSearchQuery);
    }

    private void UseSearchUnit(bool asFrom)
    {
        if (SelectedSearchUnit is null)
        {
            ErrorMessage = "Select a search result first.";
            return;
        }

        if (asFrom)
        {
            FromUnit = SelectedSearchUnit;
        }
        else
        {
            ToUnit = SelectedSearchUnit;
        }

        ErrorMessage = string.Empty;
    }

    private void ClearRecent()
    {
        if (!_pairHistory.ClearRecent())
        {
            ErrorMessage = "Recent conversion pairs are already empty.";
            return;
        }

        OnPropertyChanged(nameof(RecentPairs));
        ErrorMessage = "Recent conversion pairs cleared.";
        NotifyPersistenceStateChanged();
    }

    private async Task CopyResultAsync()
    {
        ErrorMessage = await ClipboardTextWriter.CopyAsync(_clipboardService, Result, "conversion result");
    }

    private void NotifyPairStateChanged()
    {
        OnPropertyChanged(nameof(CurrentPair));
        OnPropertyChanged(nameof(IsCurrentPairFavorite));
        OnPropertyChanged(nameof(FavoriteToggleLabel));
    }

    private void NotifyPersistenceStateChanged()
    {
        if (!_suppressPersistenceNotifications)
        {
            PersistenceStateChanged?.Invoke();
        }
    }

    private static IReadOnlyList<ConversionPair> DecodeTokens(IEnumerable<string>? tokens)
    {
        if (tokens is null)
        {
            return Array.Empty<ConversionPair>();
        }

        var pairs = new List<ConversionPair>();
        foreach (var token in tokens)
        {
            if (ConversionPairToken.TryDecode(token, out var pair) && pair is not null)
            {
                pairs.Add(pair);
            }
        }

        return pairs;
    }
}
