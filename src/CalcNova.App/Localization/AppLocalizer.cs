// src/CalcNova.App/Localization/AppLocalizer.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace CalcNova.App.Localization;

public sealed class AppLocalizer : IAppLocalizer
{
    private static readonly Lazy<AppLocalizer> _instance = new(() => new AppLocalizer());
    public static AppLocalizer Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<AppStringKey, string>> _catalogs = new();
    private string _currentCulture = "en-US";

    public event Action? LanguageChanged;

    public AppLocalizer()
    {
        // Register default catalogs
        _catalogs["en-US"] = EnglishAppStrings.Strings;
        _catalogs["hi-IN"] = HindiAppStrings.Strings;
    }

    public string CurrentCulture
    {
        get => _currentCulture;
        set
        {
            string normalized = NormalizeCulture(value);
            if (_currentCulture != normalized)
            {
                _currentCulture = normalized;
                LanguageChanged?.Invoke();
            }
        }
    }

    public string GetString(AppStringKey key)
    {
        // 1. Attempt lookup in current culture
        if (_catalogs.TryGetValue(_currentCulture, out var activeCatalog) &&
            activeCatalog.TryGetValue(key, out var localized) &&
            !string.IsNullOrWhiteSpace(localized))
        {
            return localized;
        }

        // 2. Fallback to English (en-US)
        if (_catalogs.TryGetValue("en-US", out var fallbackCatalog) &&
            fallbackCatalog.TryGetValue(key, out var fallback) &&
            !string.IsNullOrWhiteSpace(fallback))
        {
            return fallback;
        }

        // 3. Last-resort fallback: return enum key name without crashing
        return $"[{key}]";
    }

    public string this[AppStringKey key] => GetString(key);

    private static string NormalizeCulture(string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
            return "en-US";

        if (cultureName.StartsWith("hi", StringComparison.OrdinalIgnoreCase))
            return "hi-IN";

        return "en-US";
    }
}
