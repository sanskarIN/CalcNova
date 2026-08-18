namespace CalcNova.Persistence.History;

public sealed record HistoryEntry(
    long Id,
    string Expression,
    string Result,
    DateTimeOffset CreatedAt,
    bool IsFavorite);
