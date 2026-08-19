namespace CalcNova.Graphing;

public sealed record MultiGraphTableRow(
    string ExpressionId,
    string Label,
    int Segment,
    double X,
    double Y);
