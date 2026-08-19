using System.Text;

namespace CalcNova.App.Infrastructure;

public static class ExportPreviewFormatter
{
    public const int DefaultMaximumCharacters = 4_096;
    public const int DefaultMaximumLines = 80;

    private const string TruncationNotice = "… preview truncated; full content is preserved for copy/export.";

    public static string Create(
        string? content,
        int maximumCharacters = DefaultMaximumCharacters,
        int maximumLines = DefaultMaximumLines)
    {
        if (maximumCharacters <= TruncationNotice.Length + Environment.NewLine.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumCharacters),
                $"Preview character limit must exceed {TruncationNotice.Length + Environment.NewLine.Length} characters.");
        }

        if (maximumLines <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumLines), "Preview line limit must be positive.");
        }

        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        if (content.Length <= maximumCharacters && CountLines(content, maximumLines) <= maximumLines)
        {
            return content;
        }

        var contentBudget = maximumCharacters - TruncationNotice.Length - Environment.NewLine.Length;
        var builder = new StringBuilder(Math.Min(maximumCharacters, content.Length));
        using var reader = new StringReader(content);

        var lineCount = 0;
        while (lineCount < maximumLines && reader.ReadLine() is { } line)
        {
            var separatorLength = builder.Length == 0 ? 0 : Environment.NewLine.Length;
            var remaining = contentBudget - builder.Length - separatorLength;
            if (remaining <= 0)
            {
                break;
            }

            if (separatorLength > 0)
            {
                builder.Append(Environment.NewLine);
            }

            if (line.Length <= remaining)
            {
                builder.Append(line);
            }
            else
            {
                builder.Append(SafePrefix(line, remaining));
                break;
            }

            lineCount++;
        }

        if (builder.Length > 0)
        {
            builder.Append(Environment.NewLine);
        }

        builder.Append(TruncationNotice);
        return builder.ToString();
    }

    private static int CountLines(string content, int stopAfter)
    {
        var lineCount = 1;
        foreach (var character in content)
        {
            if (character != '\n')
            {
                continue;
            }

            lineCount++;
            if (lineCount > stopAfter)
            {
                return lineCount;
            }
        }

        return lineCount;
    }

    private static string SafePrefix(string value, int maximumLength)
    {
        if (maximumLength <= 0)
        {
            return string.Empty;
        }

        var length = Math.Min(maximumLength, value.Length);
        if (length < value.Length && length > 0 && char.IsHighSurrogate(value[length - 1]))
        {
            length--;
        }

        return value[..length];
    }
}
