namespace CalcNova.App.Localization;

internal static class EnglishAppStrings
{
    public static IReadOnlyDictionary<AppStringKey, string> Values { get; } =
        new Dictionary<AppStringKey, string>
        {
            [AppStringKey.AppName] = "CalcNova",
            [AppStringKey.Tagline] = "Fast. Precise. Private. Everywhere.",
            [AppStringKey.LocalFirst] = "Local-first",

            [AppStringKey.ModeCalculator] = "Calculator",
            [AppStringKey.ModeProgrammer] = "Programmer",
            [AppStringKey.ModeCodePoint] = "Unicode Code Points",
            [AppStringKey.ModeConverter] = "Converter",
            [AppStringKey.ModeStatistics] = "Statistics",
            [AppStringKey.ModeEquations] = "Equations",
            [AppStringKey.ModeMatrices] = "Matrices",
            [AppStringKey.ModeGraphing] = "Graphing",
            [AppStringKey.ModeDateTime] = "Date & Duration",
            [AppStringKey.ModeCurrency] = "Currency",
            [AppStringKey.ModeHistory] = "History",
            [AppStringKey.ModeSettings] = "Settings",
            [AppStringKey.ModeAbout] = "About",

            [AppStringKey.CalculatorTitle] = "Standard + Scientific",
            [AppStringKey.CalculatorSubtitle] = "Safe local expression evaluation",
            [AppStringKey.PromptEnterExpression] = "Enter an expression",

            [AppStringKey.ActionEvaluate] = "Evaluate",
            [AppStringKey.ActionPasteExpression] = "Paste expression",
            [AppStringKey.ActionCopyResult] = "Copy result",
            [AppStringKey.ActionConvert] = "Convert",
            [AppStringKey.ActionSwap] = "Swap",
            [AppStringKey.ActionAnalyze] = "Analyze",
            [AppStringKey.ActionSolve] = "Solve",
            [AppStringKey.ActionCopy] = "Copy",
            [AppStringKey.ActionRefresh] = "Refresh",
            [AppStringKey.ActionSave] = "Save",
            [AppStringKey.ActionReset] = "Reset",

            [AppStringKey.LabelExpression] = "Expression",
            [AppStringKey.LabelResult] = "Result",
            [AppStringKey.LabelAngleUnit] = "Angle unit",
            [AppStringKey.LabelWordSize] = "Word size",
            [AppStringKey.LabelSignificantDigits] = "Significant digits",
            [AppStringKey.LabelRecentPairs] = "Recent pairs",
            [AppStringKey.LabelFavoritePairs] = "Favorite pairs",
            [AppStringKey.LabelTheme] = "Theme",
            [AppStringKey.LabelHistoryLimit] = "History limit",

            [AppStringKey.StatusCopied] = "Copied.",
            [AppStringKey.StatusReady] = "Ready.",
            [AppStringKey.ErrorInvalidInput] = "Check the input and try again.",
            [AppStringKey.ErrorClipboardUnavailable] = "Clipboard access is unavailable on this platform."
        };
}
