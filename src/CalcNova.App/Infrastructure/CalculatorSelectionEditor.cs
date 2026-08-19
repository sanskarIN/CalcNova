namespace CalcNova.App.Infrastructure;

public sealed record CalculatorSelectionEdit(string Expression, int CaretIndex);

public static class CalculatorSelectionEditor
{
    public static CalculatorSelectionEdit ApplyToken(
        string? expression,
        int selectionStart,
        int selectionEnd,
        string token,
        int maximumLength)
    {
        expression ??= string.Empty;
        ArgumentException.ThrowIfNullOrEmpty(token);
        if (maximumLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumLength));
        }

        var start = Math.Clamp(selectionStart, 0, expression.Length);
        var end = Math.Clamp(selectionEnd, 0, expression.Length);
        if (start > end)
        {
            (start, end) = (end, start);
        }

        var hasSelection = start != end;
        if (hasSelection && IsWrapperToken(token))
        {
            var selected = expression[start..end];
            var replacement = token + selected + ")";
            EnsureWithinLimit(expression.Length - selected.Length + replacement.Length, maximumLength);
            var wrapped = expression[..start] + replacement + expression[end..];
            return new CalculatorSelectionEdit(wrapped, start + replacement.Length);
        }

        EnsureWithinLimit(expression.Length - (end - start) + token.Length, maximumLength);
        var replaced = expression[..start] + token + expression[end..];
        return new CalculatorSelectionEdit(replaced, start + token.Length);
    }

    public static bool IsWrapperToken(string token) =>
        !string.IsNullOrEmpty(token) && token.EndsWith('(');

    private static void EnsureWithinLimit(int resultLength, int maximumLength)
    {
        if (resultLength > maximumLength)
        {
            throw new InvalidOperationException("Expression limit reached.");
        }
    }
}
