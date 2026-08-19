using System.Globalization;

namespace CalcNova.App.Localization;

public interface IAppLocalizer
{
    CultureInfo Culture { get; }

    IReadOnlyList<CultureInfo> SupportedCultures { get; }

    string this[AppStringKey key] { get; }

    bool TrySetCulture(string? cultureName);
}
