namespace CalcNova.Graphing;

public static class GraphExpressionListParser
{
    public static IReadOnlyList<GraphExpressionDefinition> Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<GraphExpressionDefinition>();
        }

        var expressions = text
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        if (expressions.Length > MultiGraphSampler.MaximumExpressions)
        {
            throw new ArgumentOutOfRangeException(
                nameof(text),
                expressions.Length,
                $"A maximum of {MultiGraphSampler.MaximumExpressions} graph expressions can be supplied at once.");
        }

        return expressions
            .Select((expression, index) => new GraphExpressionDefinition(
                $"series-{index + 1}",
                $"f{index + 1}",
                expression))
            .ToArray();
    }
}
