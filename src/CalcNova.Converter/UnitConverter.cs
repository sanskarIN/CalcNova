namespace CalcNova.Converter;

public sealed class UnitConverter
{
    public double Convert(double value, string fromUnitId, string toUnitId)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Conversion input must be a finite number.");
        }

        var source = UnitCatalog.Get(fromUnitId);
        var target = UnitCatalog.Get(toUnitId);
        if (source.Category != target.Category)
        {
            throw new InvalidOperationException($"Cannot convert {source.Category} to {target.Category}.");
        }

        var baseValue = source.ToBase(value);
        var result = target.FromBase(baseValue);
        if (!double.IsFinite(result))
        {
            throw new OverflowException("The converted value is outside the supported numeric range.");
        }

        return result == 0d ? 0d : result;
    }

    public IReadOnlyList<UnitDefinition> Search(string query, UnitCategory? category = null)
    {
        query ??= string.Empty;
        IEnumerable<UnitDefinition> units = UnitCatalog.Units;
        if (category is not null)
        {
            units = units.Where(unit => unit.Category == category.Value);
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            return units.ToArray();
        }

        return units
            .Where(unit => unit.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                           unit.Symbol.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                           unit.Id.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
