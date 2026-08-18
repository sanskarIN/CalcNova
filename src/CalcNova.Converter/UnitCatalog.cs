namespace CalcNova.Converter;

public static class UnitCatalog
{
    private static readonly IReadOnlyList<UnitDefinition> AllUnits =
    [
        new("m", "Meter", "m", UnitCategory.Length, 1d),
        new("km", "Kilometer", "km", UnitCategory.Length, 1000d),
        new("cm", "Centimeter", "cm", UnitCategory.Length, 0.01d),
        new("mm", "Millimeter", "mm", UnitCategory.Length, 0.001d),
        new("in", "Inch", "in", UnitCategory.Length, 0.0254d),
        new("ft", "Foot", "ft", UnitCategory.Length, 0.3048d),
        new("yd", "Yard", "yd", UnitCategory.Length, 0.9144d),
        new("mi", "Mile", "mi", UnitCategory.Length, 1609.344d),
        new("nmi", "Nautical mile", "nmi", UnitCategory.Length, 1852d),

        new("m2", "Square meter", "m²", UnitCategory.Area, 1d),
        new("km2", "Square kilometer", "km²", UnitCategory.Area, 1_000_000d),
        new("cm2", "Square centimeter", "cm²", UnitCategory.Area, 0.0001d),
        new("ft2", "Square foot", "ft²", UnitCategory.Area, 0.09290304d),
        new("acre", "Acre", "ac", UnitCategory.Area, 4046.8564224d),
        new("ha", "Hectare", "ha", UnitCategory.Area, 10_000d),

        new("l", "Liter", "L", UnitCategory.Volume, 1d),
        new("ml", "Milliliter", "mL", UnitCategory.Volume, 0.001d),
        new("m3", "Cubic meter", "m³", UnitCategory.Volume, 1000d),
        new("floz_us", "US fluid ounce", "fl oz", UnitCategory.Volume, 0.0295735295625d),
        new("gal_us", "US gallon", "gal", UnitCategory.Volume, 3.785411784d),

        new("kg", "Kilogram", "kg", UnitCategory.Mass, 1d),
        new("g", "Gram", "g", UnitCategory.Mass, 0.001d),
        new("mg", "Milligram", "mg", UnitCategory.Mass, 0.000001d),
        new("lb", "Pound", "lb", UnitCategory.Mass, 0.45359237d),
        new("oz", "Ounce", "oz", UnitCategory.Mass, 0.028349523125d),

        new("mps", "Meter per second", "m/s", UnitCategory.Speed, 1d),
        new("kmh", "Kilometer per hour", "km/h", UnitCategory.Speed, 1d / 3.6d),
        new("mph", "Mile per hour", "mph", UnitCategory.Speed, 0.44704d),
        new("knot", "Knot", "kn", UnitCategory.Speed, 1852d / 3600d),

        new("k", "Kelvin", "K", UnitCategory.Temperature, 1d),
        new("c", "Celsius", "°C", UnitCategory.Temperature, 1d, 273.15d),
        new("f", "Fahrenheit", "°F", UnitCategory.Temperature, 5d / 9d, 255.3722222222222d),

        new("s", "Second", "s", UnitCategory.Time, 1d),
        new("min", "Minute", "min", UnitCategory.Time, 60d),
        new("h", "Hour", "h", UnitCategory.Time, 3600d),
        new("day", "Day", "d", UnitCategory.Time, 86400d),
        new("week", "Week", "wk", UnitCategory.Time, 604800d),

        new("byte", "Byte", "B", UnitCategory.Data, 1d),
        new("bit", "Bit", "bit", UnitCategory.Data, 0.125d),
        new("kb", "Kilobyte", "kB", UnitCategory.Data, 1000d),
        new("mb", "Megabyte", "MB", UnitCategory.Data, 1_000_000d),
        new("gb", "Gigabyte", "GB", UnitCategory.Data, 1_000_000_000d),
        new("kib", "Kibibyte", "KiB", UnitCategory.Data, 1024d),
        new("mib", "Mebibyte", "MiB", UnitCategory.Data, 1_048_576d),
        new("gib", "Gibibyte", "GiB", UnitCategory.Data, 1_073_741_824d),

        new("hz", "Hertz", "Hz", UnitCategory.Frequency, 1d),
        new("khz", "Kilohertz", "kHz", UnitCategory.Frequency, 1000d),
        new("mhz", "Megahertz", "MHz", UnitCategory.Frequency, 1_000_000d),
        new("ghz", "Gigahertz", "GHz", UnitCategory.Frequency, 1_000_000_000d),

        new("pa", "Pascal", "Pa", UnitCategory.Pressure, 1d),
        new("kpa", "Kilopascal", "kPa", UnitCategory.Pressure, 1000d),
        new("bar", "Bar", "bar", UnitCategory.Pressure, 100_000d),
        new("psi", "Pound per square inch", "psi", UnitCategory.Pressure, 6894.757293168d),

        new("j", "Joule", "J", UnitCategory.Energy, 1d),
        new("kj", "Kilojoule", "kJ", UnitCategory.Energy, 1000d),
        new("wh", "Watt hour", "Wh", UnitCategory.Energy, 3600d),
        new("kwh", "Kilowatt hour", "kWh", UnitCategory.Energy, 3_600_000d),

        new("w", "Watt", "W", UnitCategory.Power, 1d),
        new("kw", "Kilowatt", "kW", UnitCategory.Power, 1000d),
        new("mw_power", "Megawatt", "MW", UnitCategory.Power, 1_000_000d),

        new("n", "Newton", "N", UnitCategory.Force, 1d),
        new("kn", "Kilonewton", "kN", UnitCategory.Force, 1000d),
        new("lbf", "Pound-force", "lbf", UnitCategory.Force, 4.4482216152605d),

        new("rad", "Radian", "rad", UnitCategory.Angle, 1d),
        new("deg", "Degree", "°", UnitCategory.Angle, Math.PI / 180d),
        new("grad", "Gradian", "gon", UnitCategory.Angle, Math.PI / 200d)
    ];

    private static readonly IReadOnlyDictionary<string, UnitDefinition> ById =
        AllUnits.ToDictionary(unit => unit.Id, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<UnitDefinition> Units => AllUnits;

    public static UnitDefinition Get(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return ById.TryGetValue(id, out var unit)
            ? unit
            : throw new KeyNotFoundException($"Unknown unit id '{id}'.");
    }

    public static IReadOnlyList<UnitDefinition> ForCategory(UnitCategory category) =>
        AllUnits.Where(unit => unit.Category == category).ToArray();
}
