namespace CalcNova.Graphing;

public sealed record MultiGraphSamplingResult(
    bool Success,
    IReadOnlyList<GraphExpressionSample> Series,
    string? ErrorMessage = null)
{
    public int TotalValidPointCount => Series.Sum(item => item.ValidPointCount);

    public int TotalInvalidSampleCount => Series.Sum(item => item.InvalidSampleCount);

    public static MultiGraphSamplingResult Failed(string message) =>
        new(false, Array.Empty<GraphExpressionSample>(), message);
}
