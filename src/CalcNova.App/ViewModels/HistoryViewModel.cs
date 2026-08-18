using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Platform.History;

namespace CalcNova.App.ViewModels;

public sealed class HistoryViewModel : ViewModelBase
{
    private readonly ICalculationHistoryRepository? _repository;
    private readonly Func<int> _historyLimitProvider;
    private IReadOnlyList<HistoryEntry> _entries = Array.Empty<HistoryEntry>();
    private string _searchQuery = string.Empty;
    private string _statusMessage = string.Empty;
    private bool _isInitialized;

    public HistoryViewModel(ICalculationHistoryRepository? repository, Func<int>? historyLimitProvider = null)
    {
        _repository = repository;
        _historyLimitProvider = historyLimitProvider ?? (() => 500);
        RefreshCommand = new AsyncRelayCommand(_ => RefreshAsync());
        ClearCommand = new AsyncRelayCommand(_ => ClearAsync());
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        ToggleFavoriteCommand = new AsyncRelayCommand(ToggleFavoriteAsync);
    }

    public IReadOnlyList<HistoryEntry> Entries
    {
        get => _entries;
        private set => SetField(ref _entries, value);
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set => SetField(ref _searchQuery, value ?? string.Empty);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public bool IsAvailable => _repository is not null;

    public ICommand RefreshCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand DeleteCommand { get; }

    public ICommand ToggleFavoriteCommand { get; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_repository is null)
        {
            StatusMessage = "History storage is not configured for this platform yet.";
            return;
        }

        if (!_isInitialized)
        {
            try
            {
                await _repository.InitializeAsync(cancellationToken);
                _isInitialized = true;
            }
            catch (Exception exception)
            {
                StatusMessage = $"History could not be initialized: {exception.Message}";
                return;
            }
        }

        await RefreshAsync(cancellationToken);
    }

    public async Task RecordAsync(string expression, string result, CancellationToken cancellationToken = default)
    {
        if (_repository is null || string.IsNullOrWhiteSpace(expression) || string.IsNullOrWhiteSpace(result))
        {
            return;
        }

        if (!_isInitialized)
        {
            await InitializeAsync(cancellationToken);
            if (!_isInitialized)
            {
                return;
            }
        }

        try
        {
            await _repository.AddAsync(expression, result, cancellationToken);
            await RefreshAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            StatusMessage = $"Calculation was completed, but history could not be saved: {exception.Message}";
        }
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (_repository is null)
        {
            Entries = Array.Empty<HistoryEntry>();
            StatusMessage = "History storage is not configured for this platform yet.";
            return;
        }

        try
        {
            var limit = Math.Clamp(_historyLimitProvider(), 1, 5000);
            Entries = await _repository.GetRecentAsync(limit, SearchQuery, cancellationToken);
            StatusMessage = Entries.Count == 0 ? "No matching history entries." : string.Empty;
        }
        catch (Exception exception)
        {
            StatusMessage = $"History could not be loaded: {exception.Message}";
        }
    }

    private async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        if (_repository is null)
        {
            return;
        }

        try
        {
            await _repository.ClearAsync(cancellationToken);
            Entries = Array.Empty<HistoryEntry>();
            StatusMessage = "History cleared.";
        }
        catch (Exception exception)
        {
            StatusMessage = $"History could not be cleared: {exception.Message}";
        }
    }

    private async Task DeleteAsync(object? parameter)
    {
        if (_repository is null || parameter is not HistoryEntry entry)
        {
            return;
        }

        try
        {
            await _repository.DeleteAsync(entry.Id);
            await RefreshAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = $"History entry could not be deleted: {exception.Message}";
        }
    }

    private async Task ToggleFavoriteAsync(object? parameter)
    {
        if (_repository is null || parameter is not HistoryEntry entry)
        {
            return;
        }

        try
        {
            await _repository.SetFavoriteAsync(entry.Id, !entry.IsFavorite);
            await RefreshAsync();
        }
        catch (Exception exception)
        {
            StatusMessage = $"Favorite state could not be updated: {exception.Message}";
        }
    }
}
