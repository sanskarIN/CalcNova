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
            ["Welcome to CalcNova"] = AppStringKey.OnboardingWelcome,
            ["Calculate your way"] = AppStringKey.OnboardingCalculateTitle,
            ["Use standard and scientific calculation, Programmer, Converter, Statistics, Equations, Matrices, Graphing, Date & Duration, Currency, and History from one shared workspace."] = AppStringKey.OnboardingCalculateBody,
            ["Keyboard and touch friendly"] = AppStringKey.OnboardingInputTitle,
            ["On keyboard targets, Ctrl+PageUp and Ctrl+PageDown cycle through modes; Ctrl+Home and Ctrl+End jump to the first and last modes. Calculator number-pad input is supported when you are not editing a text field."] = AppStringKey.OnboardingInputBody,
            ["Local-first by default"] = AppStringKey.OnboardingPrivacyTitle,
            ["Calculation history and preferences stay in local app storage. Physical unit conversion is offline. Currency rates are the optional network-enhanced feature and can use cached data when available."] = AppStringKey.OnboardingPrivacyBody,
            ["No account is required for core CalcNova features. You can skip this introduction immediately."] = AppStringKey.OnboardingNoAccount,
            ["Currency Converter"] = AppStringKey.CurrencyTitle,
            ["Optional network-enhanced module. CalcNova never embeds API keys; cached rates remain usable offline."] = AppStringKey.CurrencyPrivacyBody,
            ["Calculation History"] = AppStringKey.HistoryTitle,
            ["Export visible history"] = AppStringKey.HistoryExportTitle,
            ["Exports only the currently loaded/search-matching entries and stays local until you explicitly copy it."] = AppStringKey.HistoryExportBody,
            ["Settings"] = AppStringKey.ModeSettings,
            ["Only reviewed languages are listed. English is the current source language."] = AppStringKey.SettingsLanguageReviewNote,
            ["Open-source calculator • Apache-2.0"] = AppStringKey.AboutOpenSourceLicense,
            ["Support is optional and never required to use core CalcNova features."] = AppStringKey.AboutSupportOptional,
            ["Calculations stay local by default • github.com/sanskarIN/CalcNova"] = AppStringKey.FooterLocalDefault,
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
            ["Skip"] = AppStringKey.ActionSkip,
            ["Start calculating"] = AppStringKey.ActionStartCalculating,
            ["Refresh rates"] = AppStringKey.ActionRefreshRates,
            ["Favorite"] = AppStringKey.ActionFavorite,
            ["Delete"] = AppStringKey.ActionDelete,
            ["Clear all"] = AppStringKey.ActionClearAll,
            ["Generate export preview"] = AppStringKey.ActionGenerateExportPreview,
            ["Copy export"] = AppStringKey.ActionCopyExport,
            ["Open CalcNova repository"] = AppStringKey.ActionOpenRepository,
            ["Open GitHub profile"] = AppStringKey.ActionOpenGitHubProfile,
            ["Expression"] = AppStringKey.LabelExpression,
            ["Result"] = AppStringKey.LabelResult,
            ["Angle unit"] = AppStringKey.LabelAngleUnit,
            ["Word size"] = AppStringKey.LabelWordSize,
            ["Significant digits"] = AppStringKey.LabelSignificantDigits,
            ["Recent pairs"] = AppStringKey.LabelRecentPairs,
            ["Favorite pairs"] = AppStringKey.LabelFavoritePairs,
            ["Theme"] = AppStringKey.LabelTheme,
            ["History limit"] = AppStringKey.LabelHistoryLimit,
            ["Language"] = AppStringKey.LabelLanguage,
            ["Decimal precision (1–29)"] = AppStringKey.LabelDecimalPrecisionRange,
            ["History limit (1–5000)"] = AppStringKey.LabelHistoryLimitRange,
            ["Business"] = AppStringKey.LabelBusiness,
            ["Support"] = AppStringKey.LabelSupport,
            ["Amount"] = AppStringKey.PromptAmount,
            ["From ISO code, e.g. USD"] = AppStringKey.PromptFromIsoCode,
            ["To ISO code, e.g. INR"] = AppStringKey.PromptToIsoCode,
            ["Search"] = AppStringKey.PromptSearch,
            ["Enable history"] = AppStringKey.SettingEnableHistory,
            ["Grouping separators"] = AppStringKey.SettingGroupingSeparators,
            ["Haptics on supported mobile targets"] = AppStringKey.SettingHaptics,
            ["Reduced motion"] = AppStringKey.SettingReducedMotion,
            ["High contrast preference"] = AppStringKey.SettingHighContrast
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
