using CalcNova.Persistence.History;
using Xunit;

namespace CalcNova.Persistence.Tests;

public sealed class SqliteCalculationHistoryRepositoryTests : IAsyncLifetime
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"calcnova-history-{Guid.NewGuid():N}.db");
    private SqliteCalculationHistoryRepository _repository = null!;

    public async ValueTask InitializeAsync()
    {
        _repository = new SqliteCalculationHistoryRepository(_databasePath);
        await _repository.InitializeAsync();
    }

    public ValueTask DisposeAsync()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }

        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task AddAndGetRecent_PersistsCalculation()
    {
        var added = await _repository.AddAsync("2 + 2", "4", TestContext.Current.CancellationToken);

        var entries = await _repository.GetRecentAsync(cancellationToken: TestContext.Current.CancellationToken);

        var entry = Assert.Single(entries);
        Assert.Equal(added.Id, entry.Id);
        Assert.Equal("2 + 2", entry.Expression);
        Assert.Equal("4", entry.Result);
        Assert.False(entry.IsFavorite);
    }

    [Fact]
    public async Task SetFavorite_UpdatesEntry()
    {
        var added = await _repository.AddAsync("sqrt(81)", "9", TestContext.Current.CancellationToken);

        await _repository.SetFavoriteAsync(added.Id, true, TestContext.Current.CancellationToken);
        var entries = await _repository.GetRecentAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(Assert.Single(entries).IsFavorite);
    }

    [Fact]
    public async Task Search_FiltersExpressionAndResult()
    {
        await _repository.AddAsync("10 / 4", "2.5", TestContext.Current.CancellationToken);
        await _repository.AddAsync("factorial(5)", "120", TestContext.Current.CancellationToken);

        var byExpression = await _repository.GetRecentAsync(
            query: "factorial",
            cancellationToken: TestContext.Current.CancellationToken);
        var byResult = await _repository.GetRecentAsync(
            query: "2.5",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal("factorial(5)", Assert.Single(byExpression).Expression);
        Assert.Equal("10 / 4", Assert.Single(byResult).Expression);
    }

    [Fact]
    public async Task DeleteAndClear_RemoveRows()
    {
        var first = await _repository.AddAsync("1 + 1", "2", TestContext.Current.CancellationToken);
        await _repository.AddAsync("3 + 3", "6", TestContext.Current.CancellationToken);

        await _repository.DeleteAsync(first.Id, TestContext.Current.CancellationToken);
        Assert.Single(await _repository.GetRecentAsync(cancellationToken: TestContext.Current.CancellationToken));

        await _repository.ClearAsync(TestContext.Current.CancellationToken);
        Assert.Empty(await _repository.GetRecentAsync(cancellationToken: TestContext.Current.CancellationToken));
    }
}
