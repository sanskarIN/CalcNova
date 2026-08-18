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
            historyEnabledProvider: () => Settings.HistoryEnabled);

        Settings.SettingsChanged += HandleSettingsChanged;
    }

    public event Action<AppSettings>? SettingsChanged;

    public CalculatorViewModel Calculator { get; }

    public ProgrammerViewModel Programmer { get; } = new();

    public ConverterViewModel Converter { get; } = new();

    public StatisticsViewModel Statistics { get; } = new();

    public EquationsViewModel Equations { get; } = new();

    public MatricesViewModel Matrices { get; } = new();

    public GraphingViewModel Graphing { get; } = new();

    public HistoryViewModel History { get; }

    public SettingsViewModel Settings { get; }

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
        await History.InitializeAsync(cancellationToken);
        _isInitialized = true;
    }

    private void HandleSettingsChanged(AppSettings settings)
    {
        Calculator.ApplyAngleUnit(settings.AngleUnit);
        SettingsChanged?.Invoke(settings);
    }
}
