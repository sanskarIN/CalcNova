namespace CalcNova.Converter;

public sealed record UnitDefinition(
    string Id,
    string Name,
    string Symbol,
    UnitCategory Category,
    double ToBaseFactor,
    double ToBaseOffset = 0d)
{
    public double ToBase(double value) => (value * ToBaseFactor) + ToBaseOffset;

    public double FromBase(double baseValue) => (baseValue - ToBaseOffset) / ToBaseFactor;

    public override string ToString() => $"{Name} ({Symbol})";
}
