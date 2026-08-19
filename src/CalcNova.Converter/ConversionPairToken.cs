namespace CalcNova.Converter;

public static class ConversionPairToken
{
    private const string Prefix = "v1:";
    private const char Separator = '>';

    public static string Encode(ConversionPair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);
        return $"{Prefix}{pair.FromUnitId}{Separator}{pair.ToUnitId}";
    }

    public static bool TryDecode(string? token, out ConversionPair? pair)
    {
        pair = null;
        if (string.IsNullOrWhiteSpace(token) || !token.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var payload = token[Prefix.Length..];
        var separatorIndex = payload.IndexOf(Separator);
        if (separatorIndex <= 0 || separatorIndex != payload.LastIndexOf(Separator) || separatorIndex >= payload.Length - 1)
        {
            return false;
        }

        try
        {
            pair = new ConversionPair(payload[..separatorIndex], payload[(separatorIndex + 1)..]);
            return true;
        }
        catch (Exception exception) when (exception is KeyNotFoundException or InvalidOperationException or ArgumentException)
        {
            pair = null;
            return false;
        }
    }
}
