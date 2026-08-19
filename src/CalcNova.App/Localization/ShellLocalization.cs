namespace CalcNova.App.Localization;

public static class ShellLocalization
{
    public static IReadOnlyList<AppStringKey> ModeKeys { get; } =
    [
        AppStringKey.ModeCalculator,
        AppStringKey.ModeProgrammer,
        AppStringKey.ModeCodePoint,
        AppStringKey.ModeConverter,
        AppStringKey.ModeStatistics,
        AppStringKey.ModeEquations,
        AppStringKey.ModeMatrices,
        AppStringKey.ModeGraphing,
        AppStringKey.ModeDateTime,
        AppStringKey.ModeCurrency,
        AppStringKey.ModeHistory,
        AppStringKey.ModeSettings,
        AppStringKey.ModeAbout
    ];

    public static IReadOnlyDictionary<string, AppStringKey> LiteralKeys { get; } =
        new Dictionary<string, AppStringKey>(StringComparer.Ordinal)
        {
            ["CalcNova"] = AppStringKey.AppName,
            ["Fast. Precise. Private. Everywhere."] = AppStringKey.Tagline,
            ["Local-first"] = AppStringKey.LocalFirst,
            ["Standard + Scientific"] = AppStringKey.CalculatorTitle,
            ["Safe local expression evaluation"] = AppStringKey.CalculatorSubtitle,
            ["Enter an expression"] = AppStringKey.PromptEnterExpression,
            ["Paste expression"] = AppStringKey.ActionPasteExpression,
            ["Copy result"] = AppStringKey.ActionCopyResult,
            ["Convert"] = AppStringKey.ActionConvert,
            ["Swap"] = AppStringKey.ActionSwap,
            ["Analyze"] = AppStringKey.ActionAnalyze,
            ["Solve"] = AppStringKey.ActionSolve,
            ["Copy"] = AppStringKey.ActionCopy,
            ["Refresh"] = AppStringKey.ActionRefresh,
            ["Save"] = AppStringKey.ActionSave,
            ["Reset"] = AppStringKey.ActionReset,
            ["Expression"] = AppStringKey.LabelExpression,
            ["Result"] = AppStringKey.LabelResult,
            ["Angle unit"] = AppStringKey.LabelAngleUnit,
            ["Word size"] = AppStringKey.LabelWordSize,
            ["Significant digits"] = AppStringKey.LabelSignificantDigits,
            ["Recent pairs"] = AppStringKey.LabelRecentPairs,
            ["Favorite pairs"] = AppStringKey.LabelFavoritePairs,
            ["Theme"] = AppStringKey.LabelTheme,
            ["History limit"] = AppStringKey.LabelHistoryLimit
        };

    public static IReadOnlyList<string> GetModeHeaders(IAppLocalizer localizer)
    {
        ArgumentNullException.ThrowIfNull(localizer);
        return ModeKeys.Select(key => localizer[key]).ToArray();
    }

    public static bool TryGetLiteralKey(string? literal, out AppStringKey key)
    {
        if (string.IsNullOrEmpty(literal))
        {
            key = default;
            return false;
        }

        return LiteralKeys.TryGetValue(literal, out key);
    }
}
