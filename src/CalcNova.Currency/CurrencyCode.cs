namespace CalcNova.Currency;

public static class CurrencyCode
{
    public static string Normalize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var code = value.Trim().ToUpperInvariant();
        if (code.Length != 3 || code.Any(character => character is < 'A' or > 'Z'))
        {
            throw new ArgumentException("Currency codes must contain exactly three ASCII letters.", nameof(value));
        }

        return code;
    }
}
