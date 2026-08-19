using CalcNova.Core.Evaluation;

namespace CalcNova.Platform.Settings;

public sealed record AppSettings
{
    public ThemePreference Theme { get; init; } = ThemePreference.System;

    public AngleUnit AngleUnit { get; init; } = AngleUnit.Degrees;

    public int DecimalPrecision { get; init; } = 15;

    public bool UseGroupingSeparators { get; init; } = true;

    public bool HapticsEnabled { get; init; } = true;

    public bool HistoryEnabled { get; init; } = true;

    public int HistoryLimit { get; init; } = 500;

    public bool ReducedMotion { get; init; }

    public bool HighContrast { get; init; }

    public int ConverterSignificantDigits { get; init; } = 15;

    public string[] ConverterRecentPairs { get; init; } = [];

    public string[] ConverterFavoritePairs { get; init; } = [];
}
