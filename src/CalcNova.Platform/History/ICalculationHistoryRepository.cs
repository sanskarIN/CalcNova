namespace CalcNova.Platform.History;

public interface ICalculationHistoryRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HistoryEntry>> GetRecentAsync(int limit = 100, string? query = null, CancellationToken cancellationToken = default);

    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task ClearAsync(CancellationToken cancellationToken = default);

    Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default);
}
