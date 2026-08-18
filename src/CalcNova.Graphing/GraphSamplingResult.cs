namespace CalcNova.Graphing;

public sealed record GraphSamplingResult(
    bool Success,
    IReadOnlyList<GraphSegment> Segments,
    int InvalidSampleCount,
    string? ErrorMessage)
{
    public static GraphSamplingResult Completed(IReadOnlyList<GraphSegment> segments, int invalidSampleCount) =>
        new(true, segments, invalidSampleCount, null);

    public static GraphSamplingResult Failed(string message) =>
        new(false, Array.Empty<GraphSegment>(), 0, message);
}
