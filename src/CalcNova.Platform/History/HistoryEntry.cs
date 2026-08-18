namespace CalcNova.Platform.History;

public sealed record HistoryEntry(
    long Id,
    string Expression,
    string Result,
    DateTimeOffset CreatedAt,
    bool IsFavorite);
