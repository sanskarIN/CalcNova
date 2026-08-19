namespace CalcNova.Converter;

public sealed record ConversionPair
{
    public ConversionPair(string fromUnitId, string toUnitId)
    {
        var from = UnitCatalog.Get(fromUnitId);
        var to = UnitCatalog.Get(toUnitId);
        if (from.Category != to.Category)
        {
            throw new InvalidOperationException("Conversion pairs must contain units from the same category.");
        }

        FromUnitId = from.Id;
        ToUnitId = to.Id;
        Category = from.Category;
    }

    public string FromUnitId { get; }

    public string ToUnitId { get; }

    public UnitCategory Category { get; }

    public string DisplayName
    {
        get
        {
            var from = UnitCatalog.Get(FromUnitId);
            var to = UnitCatalog.Get(ToUnitId);
            return $"{from.Symbol} → {to.Symbol}";
        }
    }

    public ConversionPair Swap() => new(ToUnitId, FromUnitId);
}
