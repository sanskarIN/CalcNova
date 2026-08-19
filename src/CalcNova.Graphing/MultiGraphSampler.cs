namespace CalcNova.Graphing;

public sealed class MultiGraphSampler
{
    public const int MaximumExpressions = 8;

    private readonly GraphSampler _sampler;

    public MultiGraphSampler(GraphSampler? sampler = null)
    {
        _sampler = sampler ?? new GraphSampler();
    }

    public MultiGraphSamplingResult Sample(
        IEnumerable<GraphExpressionDefinition> expressions,
        GraphSamplingOptions options)
    {
        ArgumentNullException.ThrowIfNull(expressions);
        ArgumentNullException.ThrowIfNull(options);

        var definitions = expressions.ToArray();
        if (definitions.Length == 0)
        {
            return MultiGraphSamplingResult.Failed("At least one graph expression is required.");
        }

        if (definitions.Length > MaximumExpressions)
        {
            return MultiGraphSamplingResult.Failed($"A maximum of {MaximumExpressions} graph expressions can be sampled at once.");
        }

        var duplicateId = definitions
            .GroupBy(definition => definition.Id, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateId is not null)
        {
            return MultiGraphSamplingResult.Failed($"Graph expression id '{duplicateId.Key}' is duplicated.");
        }

        var series = new List<GraphExpressionSample>(definitions.Length);
        foreach (var definition in definitions)
        {
            var sample = _sampler.Sample(definition.Expression, options);
            if (!sample.Success)
            {
                return MultiGraphSamplingResult.Failed(
                    $"{definition.Label}: {sample.ErrorMessage ?? "Graph sampling failed."}");
            }

            series.Add(new GraphExpressionSample(
                definition,
                sample.Segments,
                sample.InvalidSampleCount));
        }

        return new MultiGraphSamplingResult(true, series);
    }
}
