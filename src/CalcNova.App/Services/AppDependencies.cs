using CalcNova.App.Localization;
using CalcNova.Currency;
using CalcNova.Platform.Clipboard;
using CalcNova.Platform.External;
using CalcNova.Platform.Haptics;
using CalcNova.Platform.History;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Services;

public sealed record AppDependencies(
    ICalculationHistoryRepository? HistoryRepository,
    ISettingsRepository? SettingsRepository)
{
    public IExternalLinkService? ExternalLinkService { get; init; }

    public IClipboardService? ClipboardService { get; init; }

    public ICurrencyRateCache? CurrencyRateCache { get; init; }

    public ICurrencyRateProvider? CurrencyRateProvider { get; init; }

    /// <summary>
    /// The device's haptic engine. Heads without one leave this null and
    /// <see cref="NullHapticFeedbackService"/> is used, so callers never branch on availability.
    /// </summary>
    public IHapticFeedbackService? HapticFeedbackService { get; init; }

    public IAppLocalizer? Localizer { get; init; }

    public static AppDependencies Empty { get; } = new(null, null);
}
