namespace CalcNova.App.Infrastructure;

public static class CalculatorTextSymbolInput
{
    public static bool TryGetToken(string? text, out string token)
    {
        token = text switch
        {
            "×" => "*",
            "÷" => "/",
            "−" => "-",
            "–" => "-",
            "—" => "-",
            "·" => "*",
            "∙" => "*",
            _ => string.Empty
        };

        return token.Length > 0;
    }
}
