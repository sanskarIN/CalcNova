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

            [AppStringKey.OnboardingWelcome] = "Welcome to CalcNova",
            [AppStringKey.OnboardingCalculateTitle] = "Calculate your way",
            [AppStringKey.OnboardingCalculateBody] = "Use standard and scientific calculation, Programmer, Converter, Statistics, Equations, Matrices, Graphing, Date & Duration, Currency, and History from one shared workspace.",
            [AppStringKey.OnboardingInputTitle] = "Keyboard and touch friendly",
            [AppStringKey.OnboardingInputBody] = "On keyboard targets, Ctrl+PageUp and Ctrl+PageDown cycle through modes; Ctrl+Home and Ctrl+End jump to the first and last modes. Calculator number-pad input is supported when you are not editing a text field.",
            [AppStringKey.OnboardingPrivacyTitle] = "Local-first by default",
            [AppStringKey.OnboardingPrivacyBody] = "Calculation history and preferences stay in local app storage. Physical unit conversion is offline. Currency rates are the optional network-enhanced feature and can use cached data when available.",
            [AppStringKey.OnboardingNoAccount] = "No account is required for core CalcNova features. You can skip this introduction immediately.",

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
            [AppStringKey.ActionSkip] = "Skip",
            [AppStringKey.ActionStartCalculating] = "Start calculating",

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
