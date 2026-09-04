// CalcNova.Programmer/RadixConverter.cs
using System;

namespace CalcNova.Programmer;

public static class RadixConverter
{
    private const string DigitMap = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Safely parses an input string into a 64-bit unsigned integer using the specified radix.
    /// Returns false on overflow, invalid radix, or invalid digits.
    /// </summary>
    public static bool TryParse(string? text, int radix, out ulong value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text) || radix < 2 || radix > 36)
            return false;

        string clean = text.Trim().Replace(" ", "").Replace("_", "");
        if (clean.Length == 0)
            return false;

        ulong accumulator = 0;

        foreach (char c in clean)
        {
            int digitValue;
            if (c >= '0' && c <= '9') digitValue = c - '0';
            else if (c >= 'a' && c <= 'z') digitValue = c - 'a' + 10;
            else if (c >= 'A' && c <= 'Z') digitValue = c - 'A' + 10;
            else return false;

            if (digitValue >= radix)
                return false;

            // Overflow guard: verify that accumulator * radix + digitValue <= ulong.MaxValue
            if (accumulator > (ulong.MaxValue - (ulong)digitValue) / (ulong)radix)
                return false;

            accumulator = accumulator * (ulong)radix + (ulong)digitValue;
        }

        value = accumulator;
        return true;
    }

    /// <summary>
    /// Formats an unsigned 64-bit integer into a string representation for any radix (2 to 36).
    /// </summary>
    public static string Format(ulong value, int radix)
    {
        if (radix < 2 || radix > 36)
            throw new ArgumentOutOfRangeException(nameof(radix), "Radix must be between 2 and 36.");

        if (value == 0)
            return "0";

        Span<char> buffer = stackalloc char[65];
        int pos = buffer.Length;

        while (value > 0)
        {
            ulong remainder = value % (ulong)radix;
            value /= (ulong)radix;
            buffer[--pos] = DigitMap[(int)remainder];
        }

        return new string(buffer[pos..]);
    }
}
