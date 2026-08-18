using System.Collections.ObjectModel;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Graphing;

namespace CalcNova.App.ViewModels;

public sealed class GraphingWorkspaceViewModel : ViewModelBase
{
    private const int MaximumExpressions = 8;
    private readonly ObservableCollection<GraphExpressionEntryViewModel> _expressions = [];
    private IReadOnlyList<GraphSeriesModel> _series = Array.Empty<GraphSeriesModel>();
    private GraphExpressionEntryViewModel? _selectedExpression;
    private string _minimumX = "-10";
    private string _maximumX = "10";
    private string _sampleCount = "401";
    private string _summary = string.Empty;
    private string _preview = string.Empty;
    private string _errorMessage = string.Empty;
    private int _nextIdentifier = 2;

    public GraphingWorkspaceViewModel()
    {
        var primary = new GraphExpressionEntryViewModel("f1", "sin(x)");
        _expressions.Add(primary);
        _selectedExpression = primary;

        PlotCommand = new RelayCommand(_ => Plot());
        AddExpressionCommand = new RelayCommand(_ => AddExpression());
        RemoveExpressionCommand = new RelayCommand(_ => RemoveSelectedExpression());
        ShowAllCommand = new RelayCommand(_ => SetAllVisible(true));
        HideAllCommand = new RelayCommand(_ => SetAllVisible(false));
        Plot();
    }

    public ObservableCollection<GraphExpressionEntryViewModel> Expressions => _expressions;

    public GraphExpressionEntryViewModel? SelectedExpression
    {
        get => _selectedExpression;
        set => SetField(ref _selectedExpression, value);
    }

    public string Expression
    {
        get => _expressions[0].Expression;
        set
        {
            _expressions[0].Expression = value ?? string.Empty;
            OnPropertyChanged();
        }
    }

    public string MinimumX
    {
        get => _minimumX;
        set => SetField(ref _minimumX, value ?? string.Empty);
    }

    public string MaximumX
    {
        get => _maximumX;
        set => SetField(ref _maximumX, value ?? string.Empty);
    }

    public string SampleCount
    {
        get => _sampleCount;
        set => SetField(ref _sampleCount, value ?? string.Empty);
    }

    public IReadOnlyList<GraphSeriesModel> Series
    {
        get => _series;
        private set
        {
            if (SetField(ref _series, value))
            {
                OnPropertyChanged(nameof(Segments));
            }
        }
    }

    public IReadOnlyList<GraphSegment> Segments => Series.SelectMany(series => series.Segments).ToArray();

    public string Summary
    {
        get => _summary;
        private set => SetField(ref _summary, value);
    }

    public string Preview
    {
        get => _preview;
        private set => SetField(ref _preview, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand PlotCommand { get; }

    public ICommand AddExpressionCommand { get; }

    public ICommand RemoveExpressionCommand { get; }

    public ICommand ShowAllCommand { get; }

    public ICommand HideAllCommand { get; }

    public void Plot()
    {
        var rendered = new List<GraphSeriesModel>();
        var previews = new List<string>();
        var visibleCount = 0;
        var totalSegments = 0;
        var totalPoints = 0;

        foreach (var entry in Expressions)
        {
            entry.ErrorMessage = string.Empty;
            if (!entry.IsVisible || string.IsNullOrWhiteSpace(entry.Expression))
            {
                continue;
            }

            visibleCount++;
            var single = new GraphingViewModel
            {
                Expression = entry.Expression,
                MinimumX = MinimumX,
                MaximumX = MaximumX,
                SampleCount = SampleCount
            };
            single.PlotCommand.Execute(null);

            if (!string.IsNullOrWhiteSpace(single.ErrorMessage))
            {
                entry.ErrorMessage = single.ErrorMessage;
                continue;
            }

            var segments = single.Segments.ToArray();
            rendered.Add(new GraphSeriesModel(entry.Identifier, entry.Expression, segments));
            totalSegments += segments.Length;
            totalPoints += segments.Sum(segment => segment.Points.Count);
            if (!string.IsNullOrWhiteSpace(single.Preview))
            {
                previews.Add($"{entry.Identifier}: {single.Preview}");
            }
        }

        Series = rendered;
        Preview = string.Join(Environment.NewLine, previews);

        var failures = Expressions.Count(entry => entry.IsVisible && !string.IsNullOrWhiteSpace(entry.ErrorMessage));
        Summary = $"{rendered.Count}/{visibleCount} visible expression(s) plotted • {totalSegments} segment(s) • {totalPoints} point(s)";
        ErrorMessage = failures == 0
            ? string.Empty
            : $"{failures} expression(s) could not be plotted. See the expression list for details.";
    }

    private void AddExpression()
    {
        if (Expressions.Count >= MaximumExpressions)
        {
            ErrorMessage = $"A maximum of {MaximumExpressions} graph expressions can be open at once.";
            return;
        }

        var entry = new GraphExpressionEntryViewModel($"f{_nextIdentifier++}", "x");
        Expressions.Add(entry);
        SelectedExpression = entry;
        ErrorMessage = string.Empty;
    }

    private void RemoveSelectedExpression()
    {
        if (SelectedExpression is null)
        {
            ErrorMessage = "Select an expression to remove.";
            return;
        }

        if (Expressions.Count == 1)
        {
            ErrorMessage = "At least one graph expression must remain in the workspace.";
            return;
        }

        var wasPrimary = ReferenceEquals(SelectedExpression, Expressions[0]);
        Expressions.Remove(SelectedExpression);
        SelectedExpression = Expressions.FirstOrDefault();
        if (wasPrimary)
        {
            OnPropertyChanged(nameof(Expression));
        }

        ErrorMessage = string.Empty;
        Plot();
    }

    private void SetAllVisible(bool visible)
    {
        foreach (var entry in Expressions)
        {
            entry.IsVisible = visible;
        }

        Plot();
    }
}
