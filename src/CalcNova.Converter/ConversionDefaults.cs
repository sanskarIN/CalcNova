namespace CalcNova.Converter;

public static class ConversionDefaults
{
    private static readonly IReadOnlyDictionary<UnitCategory, ConversionPair> Pairs =
        new Dictionary<UnitCategory, ConversionPair>
        {
            [UnitCategory.Length] = new("m", "km"),
            [UnitCategory.Area] = new("m2", "ft2"),
            [UnitCategory.Volume] = new("l", "gal_us"),
            [UnitCategory.Mass] = new("kg", "lb"),
            [UnitCategory.Speed] = new("kmh", "mph"),
            [UnitCategory.Temperature] = new("c", "f"),
            [UnitCategory.Time] = new("h", "min"),
            [UnitCategory.Data] = new("gb", "gib"),
            [UnitCategory.Frequency] = new("hz", "khz"),
            [UnitCategory.Pressure] = new("kpa", "psi"),
            [UnitCategory.Energy] = new("j", "kj"),
            [UnitCategory.Power] = new("w", "kw"),
            [UnitCategory.Force] = new("n", "lbf"),
            [UnitCategory.Angle] = new("deg", "rad")
        };

    public static ConversionPair ForCategory(UnitCategory category)
    {
        if (!Pairs.TryGetValue(category, out var pair))
        {
            throw new ArgumentOutOfRangeException(nameof(category), category, "No default conversion pair is defined for the category.");
        }

        return pair;
    }

    public static bool TryGet(UnitCategory category, out ConversionPair? pair) =>
        Pairs.TryGetValue(category, out pair);
}
