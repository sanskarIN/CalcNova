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

            [AppStringKey.OnboardingWelcome] = "CalcNova में आपका स्वागत है",
            [AppStringKey.OnboardingCalculateTitle] = "अपने तरीके से गणना करें",
            [AppStringKey.OnboardingCalculateBody] = "एक साझा कार्यक्षेत्र से मानक और वैज्ञानिक गणना, प्रोग्रामर, कन्वर्टर, सांख्यिकी, समीकरण, मैट्रिक्स, ग्राफ, दिनांक और अवधि, मुद्रा और इतिहास का उपयोग करें।",
            [AppStringKey.OnboardingInputTitle] = "कीबोर्ड और टच के अनुकूल",
            [AppStringKey.OnboardingInputBody] = "कीबोर्ड पर Ctrl+PageUp और Ctrl+PageDown मोड बदलते हैं; Ctrl+Home और Ctrl+End पहले और अंतिम मोड पर ले जाते हैं। जब आप टेक्स्ट फ़ील्ड संपादित नहीं कर रहे हों तब कैलकुलेटर नंबर-पैड इनपुट भी समर्थित है।",
            [AppStringKey.OnboardingPrivacyTitle] = "डिफ़ॉल्ट रूप से लोकल-फर्स्ट",
            [AppStringKey.OnboardingPrivacyBody] = "गणना इतिहास और प्राथमिकताएँ स्थानीय ऐप स्टोरेज में रहती हैं। भौतिक इकाई रूपांतरण ऑफ़लाइन है। मुद्रा दरें वैकल्पिक नेटवर्क सुविधा हैं और उपलब्ध होने पर कैश डेटा का उपयोग कर सकती हैं।",
            [AppStringKey.OnboardingNoAccount] = "CalcNova की मुख्य सुविधाओं के लिए खाते की आवश्यकता नहीं है। आप इस परिचय को तुरंत छोड़ सकते हैं।",

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
            [AppStringKey.ActionSkip] = "छोड़ें",
            [AppStringKey.ActionStartCalculating] = "गणना शुरू करें",

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
