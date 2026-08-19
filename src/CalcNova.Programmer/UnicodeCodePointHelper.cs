using System.Globalization;
using System.Text;

namespace CalcNova.Programmer;

public static class UnicodeCodePointHelper
{
    public static int Parse(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var normalized = text.Trim();
        if (normalized.StartsWith("U+", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[2..];
        }
        else if (normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[2..];
        }

        if (normalized.Length is < 1 or > 6 ||
            !int.TryParse(normalized, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var codePoint) ||
            !Rune.IsValid(codePoint))
        {
            throw new FormatException("Enter a valid Unicode scalar value such as U+0041 or U+1F600.");
        }

        return codePoint;
    }

    public static string Format(int codePoint)
    {
        ValidateCodePoint(codePoint);
        return $"U+{codePoint:X4}";
    }

    public static string ToText(int codePoint)
    {
        ValidateCodePoint(codePoint);
        return new Rune(codePoint).ToString();
    }

    public static IReadOnlyList<string> GetCodePoints(string? text, int maximumRunes = 64)
    {
        if (maximumRunes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumRunes));
        }

        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<string>();
        }

        var values = new List<string>();
        foreach (var rune in text.EnumerateRunes())
        {
            if (values.Count >= maximumRunes)
            {
                throw new ArgumentException($"Text exceeds the {maximumRunes}-code-point inspection limit.", nameof(text));
            }

            values.Add(Format(rune.Value));
        }

        return values;
    }

    private static void ValidateCodePoint(int codePoint)
    {
        if (!Rune.IsValid(codePoint))
        {
            throw new ArgumentOutOfRangeException(nameof(codePoint), codePoint, "Value must be a valid Unicode scalar value.");
        }
    }
}
