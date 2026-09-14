using System.Globalization;

namespace CalcNova.App.Localization;

public interface IAppLocalizer
{
    /// <summary>
    /// Raised after <see cref="TrySetCulture"/> accepts a culture that differs from the active one.
    /// </summary>
    event Action<CultureInfo>? CultureChanged;

    CultureInfo Culture { get; }

    IReadOnlyList<CultureInfo> SupportedCultures { get; }

    string this[AppStringKey key] { get; }

    bool TrySetCulture(string? cultureName);
}
