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

    public void Record(ConversionPair pair)
    {
        ArgumentNullException.ThrowIfNull(pair);
        _recent.Remove(pair);
        _recent.Insert(0, pair);
        if (_recent.Count > _maximumRecentPairs)
        {
            _recent.RemoveRange(_maximumRecentPairs, _recent.Count - _maximumRecentPairs);
        }
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

    public void ClearRecent() => _recent.Clear();
}
