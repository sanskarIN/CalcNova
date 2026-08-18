using CalcNova.Currency;
using CalcNova.Platform.History;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Services;

public sealed record AppDependencies(
    ICalculationHistoryRepository? HistoryRepository,
    ISettingsRepository? SettingsRepository)
{
    public ICurrencyRateCache? CurrencyRateCache { get; init; }

    public ICurrencyRateProvider? CurrencyRateProvider { get; init; }

    public static AppDependencies Empty { get; } = new(null, null);
}
