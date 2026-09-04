// src/CalcNova.App/Localization/HindiAppStrings.cs
using System.Collections.Generic;

namespace CalcNova.App.Localization;

public static class HindiAppStrings
{
    public static readonly IReadOnlyDictionary<AppStringKey, string> Strings = new Dictionary<AppStringKey, string>
    {
        [AppStringKey.AppName] = "कैलकनोवा",
        [AppStringKey.StandardMode] = "मानक",
        [AppStringKey.ScientificMode] = "वैज्ञानिक",
        [AppStringKey.ProgrammerMode] = "प्रोग्रामर",
        [AppStringKey.GraphingMode] = "ग्राफ़िंग",
        [AppStringKey.ConverterMode] = "परिवर्तक",
        [AppStringKey.HistoryTitle] = "गणना इतिहास",
        [AppStringKey.ClearHistory] = "इतिहास साफ़ करें",
        [AppStringKey.SettingsTitle] = "सेटिंग्स",
        [AppStringKey.Theme] = "थीम",
        [AppStringKey.Language] = "भाषा",
        [AppStringKey.ErrorDivisionByZero] = "शून्य से विभाजन संभव नहीं",
        [AppStringKey.ErrorInvalidInput] = "अमान्य इनपुट",
        [AppStringKey.ErrorOverflow] = "ओवरफ़्लो त्रुटि",
        [AppStringKey.GraphAsymptoteNotice] = "असतत मान पाया गया",
        [AppStringKey.RadixOverflowWarning] = "मान 64-बिट सीमा से अधिक है"
    };
}
