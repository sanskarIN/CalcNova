// CalcNova.Persistence/History/SqliteCalculationHistoryRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CalcNova.Platform.History;
using Microsoft.Data.Sqlite;

namespace CalcNova.Persistence.History;

public sealed class SqliteCalculationHistoryRepository : ICalculationHistoryRepository, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private bool _initialized;

    public SqliteCalculationHistoryRepository(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        await _connection.OpenAsync();

        // Configure WAL mode and busy timeout to avoid locked errors
        using (var pragmaCmd = _connection.CreateCommand())
        {
            pragmaCmd.CommandText = @"
                PRAGMA journal_mode = WAL;
                PRAGMA busy_timeout = 5000;
                PRAGMA synchronous = NORMAL;";
            await pragmaCmd.ExecuteNonQueryAsync();
        }

        using (var createCmd = _connection.CreateCommand())
        {
            createCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS CalculationHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    Expression TEXT NOT NULL,
                    Result TEXT NOT NULL,
                    Mode TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS IX_History_Timestamp ON CalculationHistory(Timestamp DESC);";
            await createCmd.ExecuteNonQueryAsync();
        }

        _initialized = true;
    }

    public async Task AddEntryAsync(HistoryEntry entry)
    {
        await InitializeAsync();
        await _writeLock.WaitAsync();

        try
        {
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO CalculationHistory (Timestamp, Expression, Result, Mode)
                VALUES ($timestamp, $expr, $result, $mode);";

            command.Parameters.AddWithValue("$timestamp", entry.Timestamp.ToString("o"));
            command.Parameters.AddWithValue("$expr", entry.Expression);
            command.Parameters.AddWithValue("$result", entry.Result);
            command.Parameters.AddWithValue("$mode", entry.Mode);

            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public void Dispose()
    {
        _writeLock.Dispose();
        _connection.Dispose();
    }
}
