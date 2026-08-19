using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.Localization;
using CalcNova.Core.Evaluation;
using CalcNova.Platform.Settings;

namespace CalcNova.App.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsRepository? _repository;
    private readonly IAppLocalizer _localizer;
    private readonly SemaphoreSlim _saveGate = new(1, 1);
    private ThemePreference _theme = ThemePreference.System;
    private AngleUnit _angleUnit = AngleUnit.Degrees;
    private string _cultureName = "en";
    private int _decimalPrecision = 15;
    private bool _useGroupingSeparators = true;
    private bool _hapticsEnabled = true;
    private bool _historyEnabled = true;
    private int _historyLimit = 500;
    private bool _reducedMotion;
    private bool _highContrast;
    private int _converterSignificantDigits = 15;
    private string[] _converterRecentPairs = [];
    private string[] _converterFavoritePairs = [];
    private int _completedOnboardingVersion;
    private string _statusMessage = string.Empty;

    public SettingsViewModel(ISettingsRepository? repository, IAppLocalizer? localizer = null)
    {
        _repository = repository;
        _localizer = localizer ?? new AppLocalizer();
        SupportedCultureNames = _localizer.SupportedCultures
            .Select(culture => culture.Name)
            .ToArray();
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        ResetCommand = new AsyncRelayCommand(_ => ResetAsync());
        CompleteOnboardingCommand = new AsyncRelayCommand(_ => CompleteOnboardingAsync());
        SkipOnboardingCommand = new AsyncRelayCommand(_ => SkipOnboardingAsync());
    }

    public event Action<AppSettings>? SettingsChanged;

    public IReadOnlyList<ThemePreference> Themes { get; } = Enum.GetValues<ThemePreference>();

    public IReadOnlyList<AngleUnit> AngleUnits { get; } = Enum.GetValues<AngleUnit>();

    public IReadOnlyList<string> SupportedCultureNames { get; }

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

    public string CultureName
    {
        get => _cultureName;
        set => SetField(ref _cultureName, value ?? string.Empty);
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

    public int ConverterSignificantDigits => _converterSignificantDigits;

    public IReadOnlyList<string> ConverterRecentPairs => _converterRecentPairs;

    public IReadOnlyList<string> ConverterFavoritePairs => _converterFavoritePairs;

    public int CompletedOnboardingVersion => _completedOnboardingVersion;

    public bool ShouldShowOnboarding => OnboardingPolicy.ShouldShow(_completedOnboardingVersion);

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public ICommand SaveCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand CompleteOnboardingCommand { get; }

    public ICommand SkipOnboardingCommand { get; }

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
            SettingsChanged?.Invoke(CreateValidatedSettings());
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidDataException or IOException or UnauthorizedAccessException)
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
                await SaveToRepositoryAsync(settings, cancellationToken);
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

    public async Task PersistConverterStateAsync(
        int significantDigits,
        IEnumerable<string> recentPairs,
        IEnumerable<string> favoritePairs,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recentPairs);
        ArgumentNullException.ThrowIfNull(favoritePairs);
        if (significantDigits is < 1 or > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(significantDigits));
        }

        _converterSignificantDigits = significantDigits;
        _converterRecentPairs = recentPairs.Take(12).ToArray();
        _converterFavoritePairs = favoritePairs.Take(100).ToArray();

        if (_repository is null)
        {
            return;
        }

        try
        {
            await SaveToRepositoryAsync(CreateValidatedSettings(), cancellationToken);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidDataException or IOException or UnauthorizedAccessException)
        {
            StatusMessage = $"Converter preferences could not be saved: {exception.Message}";
        }
    }

    public Task CompleteOnboardingAsync(CancellationToken cancellationToken = default)
    {
        return PersistOnboardingCompletionAsync("Onboarding completed.", cancellationToken);
    }

    public Task SkipOnboardingAsync(CancellationToken cancellationToken = default)
    {
        return PersistOnboardingCompletionAsync("Onboarding skipped.", cancellationToken);
    }

    private async Task PersistOnboardingCompletionAsync(string successMessage, CancellationToken cancellationToken)
    {
        _completedOnboardingVersion = OnboardingPolicy.MarkCurrentVersionCompleted();
        OnPropertyChanged(nameof(CompletedOnboardingVersion));
        OnPropertyChanged(nameof(ShouldShowOnboarding));

        var settings = CreateValidatedSettings();
        try
        {
            if (_repository is not null)
            {
                await SaveToRepositoryAsync(settings, cancellationToken);
                StatusMessage = successMessage;
            }
            else
            {
                StatusMessage = $"{successMessage} The choice applies for this session because settings storage is unavailable.";
            }

            SettingsChanged?.Invoke(settings);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidDataException or IOException or UnauthorizedAccessException)
        {
            StatusMessage = $"Onboarding state could not be saved: {exception.Message}";
        }
    }

    private async Task ResetAsync()
    {
        var completedOnboardingVersion = _completedOnboardingVersion;
        Apply(new AppSettings { CompletedOnboardingVersion = completedOnboardingVersion });
        await SaveAsync();
    }

    private async Task SaveToRepositoryAsync(AppSettings settings, CancellationToken cancellationToken)
    {
        if (_repository is null)
        {
            return;
        }

        await _saveGate.WaitAsync(cancellationToken);
        try
        {
            await _repository.SaveAsync(settings, cancellationToken);
        }
        finally
        {
            _saveGate.Release();
        }
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

        if (_converterSignificantDigits is < 1 or > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(ConverterSignificantDigits), "Converter precision must be between 1 and 17.");
        }

        if (!_localizer.TrySetCulture(CultureName))
        {
            throw new ArgumentException("The selected application language is not supported.", nameof(CultureName));
        }

        var normalizedCultureName = _localizer.Culture.Name;
        if (!string.Equals(_cultureName, normalizedCultureName, StringComparison.Ordinal))
        {
            _cultureName = normalizedCultureName;
            OnPropertyChanged(nameof(CultureName));
        }

        return new AppSettings
        {
            Theme = Theme,
            AngleUnit = AngleUnit,
            CultureName = normalizedCultureName,
            DecimalPrecision = DecimalPrecision,
            UseGroupingSeparators = UseGroupingSeparators,
            HapticsEnabled = HapticsEnabled,
            HistoryEnabled = HistoryEnabled,
            HistoryLimit = HistoryLimit,
            ReducedMotion = ReducedMotion,
            HighContrast = HighContrast,
            ConverterSignificantDigits = _converterSignificantDigits,
            ConverterRecentPairs = _converterRecentPairs.ToArray(),
            ConverterFavoritePairs = _converterFavoritePairs.ToArray(),
            CompletedOnboardingVersion = OnboardingPolicy.NormalizeCompletedVersion(_completedOnboardingVersion)
        };
    }

    private void Apply(AppSettings settings)
    {
        Theme = settings.Theme;
        AngleUnit = settings.AngleUnit;
        ApplyCulture(settings.CultureName);
        DecimalPrecision = settings.DecimalPrecision;
        UseGroupingSeparators = settings.UseGroupingSeparators;
        HapticsEnabled = settings.HapticsEnabled;
        HistoryEnabled = settings.HistoryEnabled;
        HistoryLimit = settings.HistoryLimit;
        ReducedMotion = settings.ReducedMotion;
        HighContrast = settings.HighContrast;
        _converterSignificantDigits = settings.ConverterSignificantDigits;
        _converterRecentPairs = settings.ConverterRecentPairs?.Take(12).ToArray() ?? [];
        _converterFavoritePairs = settings.ConverterFavoritePairs?.Take(100).ToArray() ?? [];
        _completedOnboardingVersion = OnboardingPolicy.NormalizeCompletedVersion(settings.CompletedOnboardingVersion);
        OnPropertyChanged(nameof(ConverterSignificantDigits));
        OnPropertyChanged(nameof(ConverterRecentPairs));
        OnPropertyChanged(nameof(ConverterFavoritePairs));
        OnPropertyChanged(nameof(CompletedOnboardingVersion));
        OnPropertyChanged(nameof(ShouldShowOnboarding));
    }

    private void ApplyCulture(string? cultureName)
    {
        if (!_localizer.TrySetCulture(cultureName))
        {
            _localizer.TrySetCulture("en");
        }

        _cultureName = _localizer.Culture.Name;
        OnPropertyChanged(nameof(CultureName));
    }
}
