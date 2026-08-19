namespace CalcNova.Graphing;

public sealed record GraphExpressionSample(
    GraphExpressionDefinition Definition,
    IReadOnlyList<GraphSegment> Segments,
    int InvalidSampleCount)
{
    public int ValidPointCount => Segments.Sum(segment => segment.Points.Count);
}
