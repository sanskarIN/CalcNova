using CalcNova.App.Localization;
using CalcNova.App.Services;
using CalcNova.Platform.Settings;

namespace CalcNova.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    public const int ModeCount = 13;

    private bool _isInitialized;
    private int _selectedModeIndex;

    public MainViewModel(AppDependencies? dependencies = null)
    {
        dependencies ??= AppDependencies.Empty;

        Localizer = dependencies.Localizer ?? new AppLocalizer();
        Settings = new SettingsViewModel(dependencies.SettingsRepository);
        History = new HistoryViewModel(
            dependencies.HistoryRepository,
            () => Settings.HistoryLimit,
            dependencies.ClipboardService);
        Calculator = new CalculatorViewModel(
            recordCalculationAsync: (expression, result) => History.RecordAsync(expression, result),
            historyEnabledProvider: () => Settings.HistoryEnabled,
            clipboardService: dependencies.ClipboardService);
        Programmer = new ProgrammerViewModel(dependencies.ClipboardService);
        CodePoint = new CodePointViewModel(dependencies.ClipboardService);
        Converter = new ConverterViewModel(dependencies.ClipboardService);
        Statistics = new StatisticsViewModel(dependencies.ClipboardService);
        Matrices = new MatricesViewModel(dependencies.ClipboardService);
        Graphing = new GraphingViewModel(dependencies.ClipboardService);
        Currency = new CurrencyViewModel(dependencies.CurrencyRateCache, dependencies.CurrencyRateProvider);
        About = new AboutViewModel(dependencies.ExternalLinkService);

        Settings.SettingsChanged += HandleSettingsChanged;
        Converter.PersistenceStateChanged += HandleConverterPersistenceStateChanged;
    }

    public event Action<AppSettings>? SettingsChanged;

    public IAppLocalizer Localizer { get; }

    public CalculatorViewModel Calculator { get; }

    public ProgrammerViewModel Programmer { get; }

    public CodePointViewModel CodePoint { get; }

    public ConverterViewModel Converter { get; }

    public StatisticsViewModel Statistics { get; }

    public EquationsViewModel Equations { get; } = new();

    public MatricesViewModel Matrices { get; }

    public GraphingViewModel Graphing { get; }

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

    public void SelectNextMode()
    {
        SelectedModeIndex = NormalizeModeIndex(SelectedModeIndex + 1);
    }

    public void SelectPreviousMode()
    {
        SelectedModeIndex = NormalizeModeIndex(SelectedModeIndex - 1);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            return;
        }

        await Settings.LoadAsync(cancellationToken);
        await History.InitializeAsync(cancellationToken);
        _isInitialized = true;
    }

    private static int NormalizeModeIndex(int index)
    {
        var normalized = index % ModeCount;
        return normalized < 0 ? normalized + ModeCount : normalized;
    }

    private void HandleSettingsChanged(AppSettings settings)
    {
        Calculator.ApplyAngleUnit(settings.AngleUnit);
        Converter.RestorePersistedState(
            settings.ConverterRecentPairs,
            settings.ConverterFavoritePairs,
            settings.ConverterSignificantDigits);
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
