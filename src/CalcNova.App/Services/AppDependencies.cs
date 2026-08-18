using CalcNova.Platform.History;
using CalcNova.Platform.Settings;

namespace CalcNova.App.Services;

public sealed record AppDependencies(
    ICalculationHistoryRepository? HistoryRepository,
    ISettingsRepository? SettingsRepository)
{
    public static AppDependencies Empty { get; } = new(null, null);
}
