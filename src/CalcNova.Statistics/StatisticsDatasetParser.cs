using System.Globalization;

namespace CalcNova.Statistics;

public static class StatisticsDatasetParser
{
    public const int MaximumValueCount = 100_000;
    public const int MaximumInputCharacters = 2_000_000;

    private static readonly char[] Separators = [',', ';', '\n', '\r', '\t'];

    public static IReadOnlyList<double> Parse(string? text, int maximumValues = MaximumValueCount)
    {
        if (maximumValues is < 1 or > MaximumValueCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumValues),
                maximumValues,
                $"Maximum values must be between 1 and {MaximumValueCount}.");
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<double>();
        }

        if (text.Length > MaximumInputCharacters)
        {
            throw new ArgumentException(
                $"Statistics input may contain at most {MaximumInputCharacters} characters.",
                nameof(text));
        }

        var tokens = text.Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length > maximumValues)
        {
            throw new ArgumentException(
                $"Statistics input may contain at most {maximumValues} values.",
                nameof(text));
        }

        var values = new double[tokens.Length];
        for (var index = 0; index < tokens.Length; index++)
        {
            if (!double.TryParse(tokens[index], NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ||
                !double.IsFinite(value))
            {
                throw new FormatException($"'{tokens[index]}' is not a finite number.");
            }

            values[index] = value;
        }

        return values;
    }
}
