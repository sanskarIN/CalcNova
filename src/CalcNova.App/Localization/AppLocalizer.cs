using System.Globalization;

namespace CalcNova.App.Localization;

public sealed class AppLocalizer : IAppLocalizer
{
    private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en");
    private static readonly CultureInfo HindiCulture = CultureInfo.GetCultureInfo("hi");
    private static readonly IReadOnlyList<CultureInfo> Cultures = [EnglishCulture, HindiCulture];

    private CultureInfo _culture = EnglishCulture;

    static AppLocalizer()
    {
        ValidateCatalog("English", EnglishAppStrings.Values);
        ValidateCatalog("Hindi", HindiAppStrings.Values);
    }

    public AppLocalizer(string? initialCultureName = null)
    {
        if (!string.IsNullOrWhiteSpace(initialCultureName))
        {
            TrySetCulture(initialCultureName);
        }
    }

    public event Action<CultureInfo>? CultureChanged;

    public CultureInfo Culture => _culture;

    public IReadOnlyList<CultureInfo> SupportedCultures => Cultures;

    public string this[AppStringKey key] =>
        string.Equals(_culture.TwoLetterISOLanguageName, HindiCulture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)
            ? HindiAppStrings.Values[key]
            : EnglishAppStrings.Values[key];

    public bool TrySetCulture(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return false;
        }

        CultureInfo requestedCulture;
        try
        {
            requestedCulture = CultureInfo.GetCultureInfo(cultureName.Trim());
        }
        catch (CultureNotFoundException)
        {
            return false;
        }

        if (!Cultures.Any(culture => string.Equals(
                requestedCulture.TwoLetterISOLanguageName,
                culture.TwoLetterISOLanguageName,
                StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        if (string.Equals(_culture.Name, requestedCulture.Name, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        _culture = requestedCulture;
        CultureChanged?.Invoke(_culture);
        return true;
    }

    private static void ValidateCatalog(string name, IReadOnlyDictionary<AppStringKey, string> values)
    {
        var missingKeys = Enum.GetValues<AppStringKey>()
            .Where(key => !values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            .ToArray();

        if (missingKeys.Length > 0)
        {
            throw new InvalidOperationException(
                $"{name} localization catalog is missing: {string.Join(", ", missingKeys)}");
        }
    }
}
