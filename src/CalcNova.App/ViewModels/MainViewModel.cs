using CalcNova.App.Services;
using CalcNova.Platform.Settings;

namespace CalcNova.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private bool _isInitialized;
    private int _selectedModeIndex;

    public MainViewModel(AppDependencies? dependencies = null)
    {
        dependencies ??= AppDependencies.Empty;

        Settings = new SettingsViewModel(dependencies.SettingsRepository);
        History = new HistoryViewModel(dependencies.HistoryRepository, () => Settings.HistoryLimit);
        Calculator = new CalculatorViewModel(
            recordCalculationAsync: (expression, result) => History.RecordAsync(expression, result),
            historyEnabledProvider: () => Settings.HistoryEnabled,
            clipboardService: dependencies.ClipboardService);
        Converter = new ConverterViewModel();
        Currency = new CurrencyViewModel(dependencies.CurrencyRateCache, dependencies.CurrencyRateProvider);
        About = new AboutViewModel(dependencies.ExternalLinkService);

        Settings.SettingsChanged += HandleSettingsChanged;
        Converter.PersistenceStateChanged += HandleConverterPersistenceStateChanged;
    }

    public event Action<AppSettings>? SettingsChanged;

    public CalculatorViewModel Calculator { get; }

    public ProgrammerViewModel Programmer { get; } = new();

    public CodePointViewModel CodePoint { get; } = new();

    public ConverterViewModel Converter { get; }

    public StatisticsViewModel Statistics { get; } = new();

    public EquationsViewModel Equations { get; } = new();

    public MatricesViewModel Matrices { get; } = new();

    public GraphingViewModel Graphing { get; } = new();

    public DateTimeViewModel DateTime { get; } = new();

    public CurrencyViewModel Currency { get; }

    public HistoryViewModel History { get; }

    public SettingsViewModel Settings { get; }

    public AboutViewModel About { get; }

    public int SelectedModeIndex
    {
        get => _selectedModeIndex;
        set => SetField(ref _selectedModeIndex, value);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            return;
        }

        await Settings.LoadAsync(cancellationToken);
        Calculator.ApplyAngleUnit(Settings.AngleUnit);
        Converter.RestorePersistedState(
            Settings.ConverterRecentPairs,
            Settings.ConverterFavoritePairs,
            Settings.ConverterSignificantDigits);
        await History.InitializeAsync(cancellationToken);
        _isInitialized = true;
    }

    private void HandleSettingsChanged(AppSettings settings)
    {
        Calculator.ApplyAngleUnit(settings.AngleUnit);
        SettingsChanged?.Invoke(settings);
    }

    private async void HandleConverterPersistenceStateChanged()
    {
        await Settings.PersistConverterStateAsync(
            Converter.SignificantDigits,
            Converter.GetRecentPairTokens(),
            Converter.GetFavoritePairTokens());
    }
}
