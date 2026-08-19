using System.Globalization;

namespace CalcNova.App.Localization;

public sealed class AppLocalizer : IAppLocalizer
{
    private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en");
    private static readonly IReadOnlyList<CultureInfo> Cultures = [EnglishCulture];

    private CultureInfo _culture = EnglishCulture;

    static AppLocalizer()
    {
        var missingKeys = Enum.GetValues<AppStringKey>()
            .Where(key => !EnglishAppStrings.Values.ContainsKey(key))
            .ToArray();

        if (missingKeys.Length > 0)
        {
            throw new InvalidOperationException(
                $"English localization catalog is missing: {string.Join(", ", missingKeys)}");
        }
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

    public string this[AppStringKey key] => EnglishAppStrings.Values[key];

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

        if (!string.Equals(
                requestedCulture.TwoLetterISOLanguageName,
                EnglishCulture.TwoLetterISOLanguageName,
                StringComparison.OrdinalIgnoreCase))
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
}
