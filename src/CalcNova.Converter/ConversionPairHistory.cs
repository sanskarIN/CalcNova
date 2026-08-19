namespace CalcNova.Converter;

public sealed class ConversionPairHistory
{
    private readonly int _maximumRecentPairs;
    private readonly List<ConversionPair> _recent = [];
    private readonly HashSet<ConversionPair> _favorites = [];

    public ConversionPairHistory(int maximumRecentPairs = 12)
    {
        if (maximumRecentPairs <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumRecentPairs));
        }

        _maximumRecentPairs = maximumRecentPairs;
    }

    public IReadOnlyList<ConversionPair> Recent => _recent.ToArray();

    public IReadOnlyList<ConversionPair> Favorites =>
        _favorites.OrderBy(pair => pair.Category).ThenBy(pair => pair.DisplayName, StringComparer.Ordinal).ToArray();

    public bool Record(ConversionPair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);
        var wasFirst = _recent.Count > 0 && _recent[0] == pair;
        _recent.Remove(pair);
        _recent.Insert(0, pair);
        if (_recent.Count > _maximumRecentPairs)
        {
            _recent.RemoveRange(_maximumRecentPairs, _recent.Count - _maximumRecentPairs);
        }

        return !wasFirst;
    }

    public bool IsFavorite(ConversionPair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);
        return _favorites.Contains(pair);
    }

    public bool ToggleFavorite(ConversionPair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);
        if (_favorites.Remove(pair))
        {
            return false;
        }

        _favorites.Add(pair);
        return true;
    }

    public void Restore(
        IEnumerable<ConversionPair>? recentPairs,
        IEnumerable<ConversionPair>? favoritePairs)
    {
        _recent.Clear();
        _favorites.Clear();

        if (recentPairs is not null)
        {
            foreach (var pair in recentPairs.Reverse())
            {
                Record(pair);
            }
        }

        if (favoritePairs is not null)
        {
            foreach (var pair in favoritePairs)
            {
                _favorites.Add(pair);
            }
        }
    }

    public bool ClearRecent()
    {
        if (_recent.Count == 0)
        {
            return false;
        }

        _recent.Clear();
        return true;
    }
}
