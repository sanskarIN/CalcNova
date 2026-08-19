namespace CalcNova.App.Localization;

internal static class HindiAppStrings
{
    public static IReadOnlyDictionary<AppStringKey, string> Values { get; } =
        new Dictionary<AppStringKey, string>
        {
            [AppStringKey.AppName] = "CalcNova",
            [AppStringKey.Tagline] = "तेज़। सटीक। निजी। हर जगह।",
            [AppStringKey.LocalFirst] = "लोकल-फर्स्ट",

            [AppStringKey.ModeCalculator] = "कैलकुलेटर",
            [AppStringKey.ModeProgrammer] = "प्रोग्रामर",
            [AppStringKey.ModeCodePoint] = "यूनिकोड कोड पॉइंट",
            [AppStringKey.ModeConverter] = "कन्वर्टर",
            [AppStringKey.ModeStatistics] = "सांख्यिकी",
            [AppStringKey.ModeEquations] = "समीकरण",
            [AppStringKey.ModeMatrices] = "मैट्रिक्स",
            [AppStringKey.ModeGraphing] = "ग्राफ",
            [AppStringKey.ModeDateTime] = "दिनांक और अवधि",
            [AppStringKey.ModeCurrency] = "मुद्रा",
            [AppStringKey.ModeHistory] = "इतिहास",
            [AppStringKey.ModeSettings] = "सेटिंग्स",
            [AppStringKey.ModeAbout] = "परिचय",

            [AppStringKey.CalculatorTitle] = "मानक + वैज्ञानिक",
            [AppStringKey.CalculatorSubtitle] = "सुरक्षित स्थानीय अभिव्यक्ति गणना",
            [AppStringKey.PromptEnterExpression] = "अभिव्यक्ति दर्ज करें",

            [AppStringKey.ActionEvaluate] = "गणना करें",
            [AppStringKey.ActionPasteExpression] = "अभिव्यक्ति पेस्ट करें",
            [AppStringKey.ActionCopyResult] = "परिणाम कॉपी करें",
            [AppStringKey.ActionConvert] = "बदलें",
            [AppStringKey.ActionSwap] = "अदला-बदली",
            [AppStringKey.ActionAnalyze] = "विश्लेषण करें",
            [AppStringKey.ActionSolve] = "हल करें",
            [AppStringKey.ActionCopy] = "कॉपी करें",
            [AppStringKey.ActionRefresh] = "रीफ्रेश करें",
            [AppStringKey.ActionSave] = "सहेजें",
            [AppStringKey.ActionReset] = "रीसेट करें",

            [AppStringKey.LabelExpression] = "अभिव्यक्ति",
            [AppStringKey.LabelResult] = "परिणाम",
            [AppStringKey.LabelAngleUnit] = "कोण इकाई",
            [AppStringKey.LabelWordSize] = "वर्ड साइज़",
            [AppStringKey.LabelSignificantDigits] = "सार्थक अंक",
            [AppStringKey.LabelRecentPairs] = "हाल की जोड़ियाँ",
            [AppStringKey.LabelFavoritePairs] = "पसंदीदा जोड़ियाँ",
            [AppStringKey.LabelTheme] = "थीम",
            [AppStringKey.LabelHistoryLimit] = "इतिहास सीमा",

            [AppStringKey.StatusCopied] = "कॉपी हो गया।",
            [AppStringKey.StatusReady] = "तैयार।",
            [AppStringKey.ErrorInvalidInput] = "इनपुट जाँचें और फिर प्रयास करें।",
            [AppStringKey.ErrorClipboardUnavailable] = "इस प्लेटफ़ॉर्म पर क्लिपबोर्ड उपलब्ध नहीं है।"
        };
}
