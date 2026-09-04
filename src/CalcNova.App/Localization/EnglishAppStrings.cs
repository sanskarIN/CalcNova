// src/CalcNova.App/Localization/EnglishAppStrings.cs
using System.Collections.Generic;

namespace CalcNova.App.Localization;

public static class EnglishAppStrings
{
    public static readonly IReadOnlyDictionary<AppStringKey, string> Strings = new Dictionary<AppStringKey, string>
    {
        [AppStringKey.AppName] = "CalcNova",
        [AppStringKey.StandardMode] = "Standard",
        [AppStringKey.ScientificMode] = "Scientific",
        [AppStringKey.ProgrammerMode] = "Programmer",
        [AppStringKey.GraphingMode] = "Graphing",
        [AppStringKey.ConverterMode] = "Converter",
        [AppStringKey.HistoryTitle] = "Calculation History",
        [AppStringKey.ClearHistory] = "Clear History",
        [AppStringKey.SettingsTitle] = "Settings",
        [AppStringKey.Theme] = "Theme",
        [AppStringKey.Language] = "Language",
        [AppStringKey.ErrorDivisionByZero] = "Cannot divide by zero",
        [AppStringKey.ErrorInvalidInput] = "Invalid input",
        [AppStringKey.ErrorOverflow] = "Overflow error",
        [AppStringKey.GraphAsymptoteNotice] = "Discontinuous jump detected",
        [AppStringKey.RadixOverflowWarning] = "Value exceeds 64-bit boundary"
    };
}
