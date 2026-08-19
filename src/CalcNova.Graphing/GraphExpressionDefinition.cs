namespace CalcNova.Graphing;

public sealed record GraphExpressionDefinition
{
    public GraphExpressionDefinition(string id, string label, string expression)
    {
        Id = ValidateText(id, nameof(id), 32);
        Label = ValidateText(label, nameof(label), 64);
        Expression = ValidateText(expression, nameof(expression), 4096);
    }

    public string Id { get; }

    public string Label { get; }

    public string Expression { get; }

    private static string ValidateText(string value, string parameterName, int maximumLength)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        var normalized = value.Trim();
        if (normalized.Length > maximumLength)
        {
            throw new ArgumentOutOfRangeException(parameterName, normalized.Length, $"{parameterName} cannot exceed {maximumLength} characters.");
        }

        return normalized;
    }
}
