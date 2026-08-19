using System.Text;
using CalcNova.Core.Evaluation;

namespace CalcNova.Core.Parsing;

/// <summary>
/// Normalizes expression text imported from the clipboard or other external text sources.
/// This is intentionally conservative: it accepts only characters that belong to CalcNova's
/// expression language after common calculator glyphs have been normalized.
/// </summary>
public static class ExpressionTextSanitizer
{
    public static string Sanitize(string? text) =>
        Sanitize(text, EvaluationOptions.Default.MaximumExpressionLength);

    public static string Sanitize(string? text, int maximumLength)
    {
        if (maximumLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumLength));
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var trimmed = text.Trim();
        if (trimmed.StartsWith('='))
        {
            trimmed = trimmed[1..].TrimStart();
        }

        var builder = new StringBuilder(Math.Min(trimmed.Length, maximumLength));
        foreach (var character in trimmed)
        {
            AppendNormalized(builder, character);
            if (builder.Length > maximumLength)
            {
                throw new ArgumentException($"Expression exceeds the {maximumLength}-character limit.", nameof(text));
            }
        }

        return builder.ToString().Trim();
    }

    private static void AppendNormalized(StringBuilder builder, char character)
    {
        switch (character)
        {
            case '×':
            case '·':
                builder.Append('*');
                return;
            case '÷':
                builder.Append('/');
                return;
            case '−':
            case '–':
            case '—':
                builder.Append('-');
                return;
            case 'π':
                builder.Append("pi");
                return;
            case 'τ':
                builder.Append("tau");
                return;
            case '²':
                builder.Append("^2");
                return;
            case '³':
                builder.Append("^3");
                return;
            case '\r':
            case '\n':
            case '\t':
                AppendSpaceIfNeeded(builder);
                return;
        }

        if (char.IsControl(character))
        {
            throw new ArgumentException("Expression contains unsupported control characters.", nameof(character));
        }

        if (char.IsLetterOrDigit(character) ||
            character is '_' or '+' or '-' or '*' or '/' or '^' or '%' or '(' or ')' or '.' or ',' ||
            char.IsWhiteSpace(character))
        {
            builder.Append(character);
            return;
        }

        throw new ArgumentException($"Expression contains an unsupported character: '{character}'.", nameof(character));
    }

    private static void AppendSpaceIfNeeded(StringBuilder builder)
    {
        if (builder.Length > 0 && !char.IsWhiteSpace(builder[^1]))
        {
            builder.Append(' ');
        }
    }
}
