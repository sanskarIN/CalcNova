using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Evaluation;
using CalcNova.Platform.Settings;

namespace CalcNova.App.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsRepository? _repository;
    private ThemePreference _theme = ThemePreference.System;
    private AngleUnit _angleUnit = AngleUnit.Degrees;
    private int _decimalPrecision = 15;
    private bool _useGroupingSeparators = true;
    private bool _hapticsEnabled = true;
    private bool _historyEnabled = true;
    private int _historyLimit = 500;
    private bool _reducedMotion;
    private bool _highContrast;
    private string _statusMessage = string.Empty;

    public SettingsViewModel(ISettingsRepository? repository)
    {
        _repository = repository;
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        ResetCommand = new AsyncRelayCommand(_ => ResetAsync());
    }

    public event Action<AppSettings>? SettingsChanged;

    public IReadOnlyList<ThemePreference> Themes { get; } = Enum.GetValues<ThemePreference>();

    public IReadOnlyList<AngleUnit> AngleUnits { get; } = Enum.GetValues<AngleUnit>();

    public ThemePreference Theme
    {
        get => _theme;
        set => SetField(ref _theme, value);
    }

    public AngleUnit AngleUnit
    {
        get => _angleUnit;
        set => SetField(ref _angleUnit, value);
    }

    public int DecimalPrecision
    {
        get => _decimalPrecision;
        set => SetField(ref _decimalPrecision, value);
    }

    public bool UseGroupingSeparators
    {
        get => _useGroupingSeparators;
        set => SetField(ref _useGroupingSeparators, value);
    }

    public bool HapticsEnabled
    {
        get => _hapticsEnabled;
        set => SetField(ref _hapticsEnabled, value);
    }

    public bool HistoryEnabled
    {
        get => _historyEnabled;
        set => SetField(ref _historyEnabled, value);
    }

    public int HistoryLimit
    {
        get => _historyLimit;
        set => SetField(ref _historyLimit, value);
    }

    public bool ReducedMotion
    {
        get => _reducedMotion;
        set => SetField(ref _reducedMotion, value);
    }

    public bool HighContrast
    {
        get => _highContrast;
        set => SetField(ref _highContrast, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public ICommand SaveCommand { get; }

    public ICommand ResetCommand { get; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = _repository is null
                ? new AppSettings()
                : await _repository.LoadAsync(cancellationToken);
            Apply(settings);
            StatusMessage = _repository is null
                ? "Settings storage is not configured for this platform yet; defaults are active."
                : string.Empty;
            SettingsChanged?.Invoke(settings);
        }
        catch (Exception exception)
        {
            StatusMessage = $"Settings could not be loaded: {exception.Message}";
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = CreateValidatedSettings();
            if (_repository is not null)
            {
                await _repository.SaveAsync(settings, cancellationToken);
                StatusMessage = "Settings saved.";
            }
            else
            {
                StatusMessage = "Settings apply for this session, but persistent storage is not configured for this platform yet.";
            }

            SettingsChanged?.Invoke(settings);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidDataException or IOException or UnauthorizedAccessException)
        {
            StatusMessage = $"Settings could not be saved: {exception.Message}";
        }
    }

    private async Task ResetAsync()
    {
        Apply(new AppSettings());
        await SaveAsync();
    }

    private AppSettings CreateValidatedSettings()
    {
        if (DecimalPrecision is < 1 or > 29)
        {
            throw new ArgumentOutOfRangeException(nameof(DecimalPrecision), "Decimal precision must be between 1 and 29.");
        }

        if (HistoryLimit is < 1 or > 5000)
        {
            throw new ArgumentOutOfRangeException(nameof(HistoryLimit), "History limit must be between 1 and 5000.");
        }

        return new AppSettings
        {
            Theme = Theme,
            AngleUnit = AngleUnit,
            DecimalPrecision = DecimalPrecision,
            UseGroupingSeparators = UseGroupingSeparators,
            HapticsEnabled = HapticsEnabled,
            HistoryEnabled = HistoryEnabled,
            HistoryLimit = HistoryLimit,
            ReducedMotion = ReducedMotion,
            HighContrast = HighContrast
        };
    }

    private void Apply(AppSettings settings)
    {
        Theme = settings.Theme;
        AngleUnit = settings.AngleUnit;
        DecimalPrecision = settings.DecimalPrecision;
        UseGroupingSeparators = settings.UseGroupingSeparators;
        HapticsEnabled = settings.HapticsEnabled;
        HistoryEnabled = settings.HistoryEnabled;
        HistoryLimit = settings.HistoryLimit;
        ReducedMotion = settings.ReducedMotion;
        HighContrast = settings.HighContrast;
    }
}
