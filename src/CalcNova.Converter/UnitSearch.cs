namespace CalcNova.Converter;

public static class UnitSearch
{
    public static IReadOnlyList<UnitDefinition> Search(
        UnitCategory category,
        string? query,
        int maximumResults = 20)
    {
        if (maximumResults is < 1 or > 200)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumResults), maximumResults, "Maximum results must be between 1 and 200.");
        }

        var units = UnitCatalog.ForCategory(category);
        if (string.IsNullOrWhiteSpace(query))
        {
            return units.Take(maximumResults).ToArray();
        }

        var normalized = query.Trim();
        return units
            .Where(unit =>
                unit.Id.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                unit.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                unit.Symbol.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(unit => unit.Id.Equals(normalized, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(unit => unit.Symbol.Equals(normalized, StringComparison.OrdinalIgnoreCase))
            .ThenBy(unit => unit.Name, StringComparer.OrdinalIgnoreCase)
            .Take(maximumResults)
            .ToArray();
    }
}
