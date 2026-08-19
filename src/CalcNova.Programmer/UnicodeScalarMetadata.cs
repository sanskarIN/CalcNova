namespace CalcNova.Programmer;

public sealed record UnicodeScalarMetadata(
    int Value,
    string CodePoint,
    string Text,
    int Plane,
    string GeneralCategory,
    int Utf8ByteCount,
    int Utf16CodeUnitCount)
{
    public string CompactSummary =>
        $"{CodePoint} • plane {Plane} • {GeneralCategory} • UTF-8 {Utf8ByteCount} byte(s) • UTF-16 {Utf16CodeUnitCount} unit(s)";
}
