using System.Globalization;
using CalcNova.Platform.History;
using Microsoft.Data.Sqlite;

namespace CalcNova.Persistence.History;

public sealed class SqliteCalculationHistoryRepository : ICalculationHistoryRepository
{
    private readonly string _connectionString;

    public SqliteCalculationHistoryRepository(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        var fullPath = Path.GetFullPath(databasePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS calculation_history (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                expression TEXT NOT NULL,
                result TEXT NOT NULL,
                created_at TEXT NOT NULL,
                is_favorite INTEGER NOT NULL DEFAULT 0
            );
            CREATE INDEX IF NOT EXISTS idx_calculation_history_created_at
                ON calculation_history(created_at DESC);
            CREATE INDEX IF NOT EXISTS idx_calculation_history_favorite
                ON calculation_history(is_favorite, created_at DESC);
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<HistoryEntry> AddAsync(string expression, string result, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(result);

        var createdAt = DateTimeOffset.UtcNow;
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var insert = connection.CreateCommand();
        insert.CommandText = """
            INSERT INTO calculation_history(expression, result, created_at, is_favorite)
            VALUES ($expression, $result, $createdAt, 0);
            """;
        insert.Parameters.AddWithValue("$expression", expression);
        insert.Parameters.AddWithValue("$result", result);
        insert.Parameters.AddWithValue("$createdAt", createdAt.ToString("O", CultureInfo.InvariantCulture));
        await insert.ExecuteNonQueryAsync(cancellationToken);

        await using var idCommand = connection.CreateCommand();
        idCommand.CommandText = "SELECT last_insert_rowid();";
        var scalar = await idCommand.ExecuteScalarAsync(cancellationToken);
        var id = Convert.ToInt64(scalar, CultureInfo.InvariantCulture);

        return new HistoryEntry(id, expression, result, createdAt, false);
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

        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        var hasQuery = !string.IsNullOrWhiteSpace(query);
        command.CommandText = hasQuery
            ? """
                SELECT id, expression, result, created_at, is_favorite
                FROM calculation_history
                WHERE expression LIKE $query OR result LIKE $query
                ORDER BY created_at DESC
                LIMIT $limit;
                """
            : """
                SELECT id, expression, result, created_at, is_favorite
                FROM calculation_history
                ORDER BY created_at DESC
                LIMIT $limit;
                """;

        if (hasQuery)
        {
            command.Parameters.AddWithValue("$query", $"%{query!.Trim()}%");
        }

        command.Parameters.AddWithValue("$limit", limit);

        var entries = new List<HistoryEntry>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            entries.Add(new HistoryEntry(
                reader.GetInt64(0),
                reader.GetString(1),
                reader.GetString(2),
                DateTimeOffset.Parse(reader.GetString(3), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                reader.GetBoolean(4)));
        }

        return entries;
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM calculation_history WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM calculation_history;";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task SetFavoriteAsync(long id, bool isFavorite, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE calculation_history SET is_favorite = $favorite WHERE id = $id;";
        command.Parameters.AddWithValue("$favorite", isFavorite ? 1 : 0);
        command.Parameters.AddWithValue("$id", id);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(_connectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}
