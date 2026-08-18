using System.Globalization;
using System.Numerics;

namespace CalcNova.App.Services;

public static class CalculatorResultFormatter
{
    public static string Format(
        string canonicalResult,
        int significantDigits,
        bool useGroupingSeparators,
        CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(canonicalResult) || canonicalResult == "Error")
        {
            return canonicalResult;
        }

        if (significantDigits is < 1 or > 29)
        {
            throw new ArgumentOutOfRangeException(nameof(significantDigits), significantDigits, "Significant digits must be between 1 and 29.");
        }

        culture ??= CultureInfo.CurrentCulture;
        var invariant = CultureInfo.InvariantCulture;
        string normalized;

        if (!canonicalResult.Contains('.') &&
            !canonicalResult.Contains('e') &&
            !canonicalResult.Contains('E') &&
            BigInteger.TryParse(canonicalResult, NumberStyles.Integer, invariant, out var integer))
        {
            normalized = integer.ToString(invariant);
        }
        else if (decimal.TryParse(canonicalResult, NumberStyles.Float, invariant, out var decimalValue))
        {
            normalized = decimalValue.ToString($"G{significantDigits}", invariant);
        }
        else if (double.TryParse(canonicalResult, NumberStyles.Float, invariant, out var doubleValue) && double.IsFinite(doubleValue))
        {
            normalized = doubleValue.ToString($"G{Math.Min(significantDigits, 17)}", invariant);
        }
        else
        {
            return canonicalResult;
        }

        return Localize(normalized, useGroupingSeparators, culture.NumberFormat);
    }

    private static string Localize(string invariantNumber, bool useGroupingSeparators, NumberFormatInfo numberFormat)
    {
        var exponentIndex = invariantNumber.IndexOfAny(['e', 'E']);
        var mantissa = exponentIndex >= 0 ? invariantNumber[..exponentIndex] : invariantNumber;
        var exponent = exponentIndex >= 0 ? invariantNumber[exponentIndex..] : string.Empty;
        var negative = mantissa.StartsWith('-', StringComparison.Ordinal);
        var unsignedMantissa = negative ? mantissa[1..] : mantissa;
        var decimalIndex = unsignedMantissa.IndexOf('.');
        var integerPart = decimalIndex >= 0 ? unsignedMantissa[..decimalIndex] : unsignedMantissa;
        var fractionalPart = decimalIndex >= 0 ? unsignedMantissa[(decimalIndex + 1)..] : string.Empty;

        if (useGroupingSeparators && exponentIndex < 0)
        {
            integerPart = ApplyGrouping(integerPart, numberFormat);
        }

        var sign = negative ? numberFormat.NegativeSign : string.Empty;
        var fraction = fractionalPart.Length == 0 ? string.Empty : numberFormat.NumberDecimalSeparator + fractionalPart;
        return sign + integerPart + fraction + exponent;
    }

    private static string ApplyGrouping(string digits, NumberFormatInfo numberFormat)
    {
        if (digits.Length <= 3 || numberFormat.NumberGroupSizes.Length == 0 || numberFormat.NumberGroupSizes[0] <= 0)
        {
            return digits;
        }

        var groups = new List<string>();
        var index = digits.Length;
        var groupSizeIndex = 0;
        var currentGroupSize = numberFormat.NumberGroupSizes[groupSizeIndex];

        while (index > 0 && currentGroupSize > 0)
        {
            var start = Math.Max(0, index - currentGroupSize);
            groups.Add(digits[start..index]);
            index = start;

            if (groupSizeIndex < numberFormat.NumberGroupSizes.Length - 1 && numberFormat.NumberGroupSizes[groupSizeIndex + 1] > 0)
            {
                groupSizeIndex++;
                currentGroupSize = numberFormat.NumberGroupSizes[groupSizeIndex];
            }
        }

        if (index > 0)
        {
            groups.Add(digits[..index]);
        }

        groups.Reverse();
        return string.Join(numberFormat.NumberGroupSeparator, groups);
    }
}
