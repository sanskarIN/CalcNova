using CalcNova.Converter;

namespace CalcNova.App.ViewModels;

public sealed record ConversionPairOption(UnitDefinition From, UnitDefinition To)
{
    public string Label => $"{From.Symbol} → {To.Symbol}";

    public override string ToString() => $"{From.Name} ({From.Symbol}) → {To.Name} ({To.Symbol})";
}
