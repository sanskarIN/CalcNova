namespace CalcNova.Graphing;

public sealed record GraphTraceResult(int Segment, double RequestedX, double X, double Y)
{
    public double Distance => Math.Abs(X - RequestedX);
}
