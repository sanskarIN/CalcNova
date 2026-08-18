using System.Text.Json;
using CalcNova.Platform.History;

namespace CalcNova.Browser.Services;

public sealed class BrowserHistoryRepository : ICalculationHistoryRepository
{
    private const string StorageKey = "calcnova.history.v1";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task InitializeAsync(CancellationToken cancellationToken = default) =>
        await BrowserInterop.EnsureInitializedAsync(cancellationToken);

    public async Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(result);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await BrowserInterop.EnsureInitializedAsync(cancellationToken);
            var entries = LoadEntries();
            var id = entries.Count == 0 ? 1 : checked(entries.Max(entry => entry.Id) + 1);
            var entry = new HistoryEntry(id, expression, result, DateTimeOffset.UtcNow, false);
            entries.Insert(0, entry);
            if (entries.Count > 5000)
            {
                entries.RemoveRange(5000, entries.Count - 5000);
            }

            SaveEntries(entries);
            return entry;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<HistoryEntry>> GetRecentAsync(
        int limit = 100,
        string? query = null,
        CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 5000)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "History limit must be between 1 and 5000.");
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await BrowserInterop.EnsureInitializedAsync(cancellationToken);
            IEnumerable<HistoryEntry> entries = LoadEntries();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var search = query.Trim();
                entries = entries.Where(entry =>
                    entry.Expression.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    entry.Result.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return entries
                .OrderByDescending(entry => entry.CreatedAt)
                .Take(limit)
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await MutateAsync(entries => entries.RemoveAll(entry => entry.Id == id) > 0, cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await BrowserInterop.EnsureInitializedAsync(cancellationToken);
            BrowserInterop.RemoveItem(StorageKey);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default)
    {
        await MutateAsync(entries =>
        {
            var index = entries.FindIndex(entry => entry.Id == id);
            if (index < 0)
            {
                return false;
            }

            entries[index] = entries[index] with { IsFavorite = isFavorite };
            return true;
        }, cancellationToken);
    }

    private async Task MutateAsync(Func<List<HistoryEntry>, bool> mutation, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await BrowserInterop.EnsureInitializedAsync(cancellationToken);
            var entries = LoadEntries();
            if (mutation(entries))
            {
                SaveEntries(entries);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private static List<HistoryEntry> LoadEntries()
    {
        var json = BrowserInterop.GetItem(StorageKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<HistoryEntry>>(json, SerializerOptions) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static void SaveEntries(IReadOnlyList<HistoryEntry> entries)
    {
        var json = JsonSerializer.Serialize(entries, SerializerOptions);
        BrowserInterop.SetItem(StorageKey, json);
    }
}
