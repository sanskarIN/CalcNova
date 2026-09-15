using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using CalcNova.App.Infrastructure;
using CalcNova.Graphing;

namespace CalcNova.App.Controls;

public sealed class GraphPlotControl : Control
{
    public static readonly StyledProperty<IReadOnlyList<GraphSegment>?> SegmentsProperty =
        AvaloniaProperty.Register<GraphPlotControl, IReadOnlyList<GraphSegment>?>(nameof(Segments));

    public static readonly StyledProperty<IReadOnlyList<GraphExpressionSample>?> SeriesProperty =
        AvaloniaProperty.Register<GraphPlotControl, IReadOnlyList<GraphExpressionSample>?>(nameof(Series));

    public static readonly StyledProperty<string> CoordinateTextProperty =
        AvaloniaProperty.Register<GraphPlotControl, string>(nameof(CoordinateText), string.Empty);

    private const double MinimumSpan = 1e-9d;
    private const double MaximumSpan = 1e15d;
    private const double AxisPadding = 0.08d;
    private const double KeyboardPanFraction = 0.10d;

    private double _minimumX = -10d;
    private double _maximumX = 10d;
    private double _minimumY = -10d;
    private double _maximumY = 10d;
    private Point _lastPointerPosition;
    private bool _isPanning;

    static GraphPlotControl()
    {
        AffectsRender<GraphPlotControl>(SegmentsProperty, SeriesProperty);
    }

    public GraphPlotControl()
    {
        ClipToBounds = true;
        Focusable = true;
        DoubleTapped += (_, _) => FitToData();
    }

    public IReadOnlyList<GraphSegment>? Segments
    {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public IReadOnlyList<GraphExpressionSample>? Series
    {
        get => GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    public string CoordinateText
    {
        get => GetValue(CoordinateTextProperty);
        private set => SetValue(CoordinateTextProperty, value);
    }

    public GraphViewport Viewport => new(_minimumX, _maximumX, _minimumY, _maximumY);

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        if (bounds.Width <= 1d || bounds.Height <= 1d)
        {
            return;
        }

        var themeForeground = Foreground ?? Brushes.Gray;
        var subtlePen = new Pen(new SolidColorBrush(Color.FromArgb(42, 127, 127, 127)), 1d);
        var axisPen = new Pen(new SolidColorBrush(Color.FromArgb(130, 127, 127, 127)), 1.2d);
        var curvePen = new Pen(themeForeground, 2d, lineCap: PenLineCap.Round, lineJoin: PenLineJoin.Round);

        DrawGrid(context, bounds, subtlePen, axisPen);

        if (Series is { Count: > 0 })
        {
            for (var seriesIndex = 0; seriesIndex < Series.Count; seriesIndex++)
            {
                DrawSegments(
                    context,
                    bounds,
                    curvePen,
                    Series[seriesIndex].Segments,
                    GraphSeriesLinePatternCatalog.ForSeriesIndex(seriesIndex));
            }
        }
        else
        {
            DrawSegments(context, bounds, curvePen, Segments, GraphSeriesLinePattern.Solid);
        }
    }

    public void ResetViewport()
    {
        _minimumX = -10d;
        _maximumX = 10d;
        _minimumY = -10d;
        _maximumY = 10d;
        InvalidateVisual();
    }

    public void FitToData()
    {
        var points = GetRenderableSegments()
            .SelectMany(segment => segment.Points)
            .Where(point => double.IsFinite(point.X) && double.IsFinite(point.Y))
            .ToArray();

        if (points.Length == 0)
        {
            ResetViewport();
            return;
        }

        var minimumX = points.Min(point => point.X);
        var maximumX = points.Max(point => point.X);
        var minimumY = points.Min(point => point.Y);
        var maximumY = points.Max(point => point.Y);

        NormalizeRange(ref minimumX, ref maximumX);
        NormalizeRange(ref minimumY, ref maximumY);

        var xPadding = (maximumX - minimumX) * AxisPadding;
        var yPadding = (maximumY - minimumY) * AxisPadding;
        _minimumX = minimumX - xPadding;
        _maximumX = maximumX + xPadding;
        _minimumY = minimumY - yPadding;
        _maximumY = maximumY + yPadding;
        InvalidateVisual();
    }

    public void PanLeft() => PanViewport(-KeyboardPanFraction, 0d);

    public void PanRight() => PanViewport(KeyboardPanFraction, 0d);

    public void PanUp() => PanViewport(0d, KeyboardPanFraction);

    public void PanDown() => PanViewport(0d, -KeyboardPanFraction);

    public void ZoomIn() => ZoomAround(ViewportCenter(), 0.82d);

    public void ZoomOut() => ZoomAround(ViewportCenter(), 1.22d);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SegmentsProperty || change.Property == SeriesProperty)
        {
            FitToData();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs eventArgs)
    {
        base.OnPointerPressed(eventArgs);
        var point = eventArgs.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        Focus();
        _isPanning = true;
        _lastPointerPosition = eventArgs.GetPosition(this);
        eventArgs.Pointer.Capture(this);
        eventArgs.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs eventArgs)
    {
        base.OnPointerMoved(eventArgs);
        var current = eventArgs.GetPosition(this);

        if (_isPanning)
        {
            PanByPixels(current.X - _lastPointerPosition.X, current.Y - _lastPointerPosition.Y);
            _lastPointerPosition = current;
            eventArgs.Handled = true;
        }

        CoordinateText = FormatCoordinate(ScreenToData(current));
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs eventArgs)
    {
        base.OnPointerReleased(eventArgs);
        if (_isPanning)
        {
            _isPanning = false;
            eventArgs.Pointer.Capture(null);
            eventArgs.Handled = true;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs eventArgs)
    {
        base.OnPointerCaptureLost(eventArgs);
        _isPanning = false;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs eventArgs)
    {
        base.OnPointerWheelChanged(eventArgs);
        if (eventArgs.Delta.Y == 0d)
        {
            return;
        }

        var anchor = ScreenToData(eventArgs.GetPosition(this));
        var factor = eventArgs.Delta.Y > 0d ? 0.82d : 1.22d;
        ZoomAround(anchor, factor);
        eventArgs.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs eventArgs)
    {
        base.OnKeyDown(eventArgs);

        var action = GraphKeyboardInput.GetAction(eventArgs.Key, eventArgs.KeyModifiers);
        if (action == GraphKeyboardAction.None)
        {
            return;
        }

        ApplyKeyboardAction(action);
        eventArgs.Handled = true;
    }

    private void ApplyKeyboardAction(GraphKeyboardAction action)
    {
        switch (action)
        {
            case GraphKeyboardAction.PanLeft:
                PanLeft();
                break;
            case GraphKeyboardAction.PanRight:
                PanRight();
                break;
            case GraphKeyboardAction.PanUp:
                PanUp();
                break;
            case GraphKeyboardAction.PanDown:
                PanDown();
                break;
            case GraphKeyboardAction.ZoomIn:
                ZoomIn();
                break;
            case GraphKeyboardAction.ZoomOut:
                ZoomOut();
                break;
            case GraphKeyboardAction.ResetViewport:
                ResetViewport();
                break;
            case GraphKeyboardAction.FitToData:
                FitToData();
                break;
        }
    }

    private void DrawGrid(DrawingContext context, Rect bounds, Pen subtlePen, Pen axisPen)
    {
        const int divisions = 10;
        for (var index = 1; index < divisions; index++)
        {
            var x = bounds.Width * index / divisions;
            var y = bounds.Height * index / divisions;
            context.DrawLine(subtlePen, new Point(x, 0d), new Point(x, bounds.Height));
            context.DrawLine(subtlePen, new Point(0d, y), new Point(bounds.Width, y));
        }

        if (_minimumX <= 0d && _maximumX >= 0d)
        {
            var xAxis = DataToScreen(new GraphPoint(0d, _minimumY)).X;
            context.DrawLine(axisPen, new Point(xAxis, 0d), new Point(xAxis, bounds.Height));
        }

        if (_minimumY <= 0d && _maximumY >= 0d)
        {
            var yAxis = DataToScreen(new GraphPoint(_minimumX, 0d)).Y;
            context.DrawLine(axisPen, new Point(0d, yAxis), new Point(bounds.Width, yAxis));
        }
    }

    private void DrawSegments(
        DrawingContext context,
        Rect bounds,
        Pen curvePen,
        IReadOnlyList<GraphSegment>? segments,
        GraphSeriesLinePattern pattern)
    {
        if (segments is null)
        {
            return;
        }

        foreach (var segment in segments)
        {
            Point? previous = null;
            var edgeIndex = 0;

            foreach (var point in segment.Points)
            {
                if (!double.IsFinite(point.X) || !double.IsFinite(point.Y))
                {
                    previous = null;
                    edgeIndex = 0;
                    continue;
                }

                var screen = DataToScreen(point);
                if (previous is not null)
                {
                    if (GraphSeriesLinePatternCatalog.ShouldDrawEdge(pattern, edgeIndex) &&
                        SegmentMayIntersectBounds(previous.Value, screen, bounds))
                    {
                        context.DrawLine(curvePen, previous.Value, screen);
                    }

                    edgeIndex++;
                }

                previous = screen;
            }
        }
    }

    private IEnumerable<GraphSegment> GetRenderableSegments()
    {
        if (Series is { Count: > 0 })
        {
            foreach (var series in Series)
            {
                foreach (var segment in series.Segments)
                {
                    yield return segment;
                }
            }

            yield break;
        }

        if (Segments is null)
        {
            yield break;
        }

        foreach (var segment in Segments)
        {
            yield return segment;
        }
    }

    private Point DataToScreen(GraphPoint point)
    {
        var width = Math.Max(1d, Bounds.Width);
        var height = Math.Max(1d, Bounds.Height);
        var x = (point.X - _minimumX) / (_maximumX - _minimumX) * width;
        var y = height - ((point.Y - _minimumY) / (_maximumY - _minimumY) * height);
        return new Point(x, y);
    }

    private GraphPoint ScreenToData(Point point)
    {
        var width = Math.Max(1d, Bounds.Width);
        var height = Math.Max(1d, Bounds.Height);
        var x = _minimumX + ((point.X / width) * (_maximumX - _minimumX));
        var y = _maximumY - ((point.Y / height) * (_maximumY - _minimumY));
        return new GraphPoint(x, y);
    }

    private void PanByPixels(double deltaX, double deltaY)
    {
        var width = Math.Max(1d, Bounds.Width);
        var height = Math.Max(1d, Bounds.Height);
        var xDelta = -(deltaX / width) * (_maximumX - _minimumX);
        var yDelta = (deltaY / height) * (_maximumY - _minimumY);

        _minimumX += xDelta;
        _maximumX += xDelta;
        _minimumY += yDelta;
        _maximumY += yDelta;
        InvalidateVisual();
    }

    private void PanViewport(double xFraction, double yFraction)
    {
        var xDelta = (_maximumX - _minimumX) * xFraction;
        var yDelta = (_maximumY - _minimumY) * yFraction;
        _minimumX += xDelta;
        _maximumX += xDelta;
        _minimumY += yDelta;
        _maximumY += yDelta;
        InvalidateVisual();
    }

    private GraphPoint ViewportCenter() => new(
        (_minimumX + _maximumX) / 2d,
        (_minimumY + _maximumY) / 2d);

    private void ZoomAround(GraphPoint anchor, double factor)
    {
        var currentXSpan = _maximumX - _minimumX;
        var currentYSpan = _maximumY - _minimumY;
        var nextXSpan = Math.Clamp(currentXSpan * factor, MinimumSpan, MaximumSpan);
        var nextYSpan = Math.Clamp(currentYSpan * factor, MinimumSpan, MaximumSpan);
        if (nextXSpan == currentXSpan && nextYSpan == currentYSpan)
        {
            return;
        }

        var xRatio = (anchor.X - _minimumX) / currentXSpan;
        var yRatio = (anchor.Y - _minimumY) / currentYSpan;
        _minimumX = anchor.X - (nextXSpan * xRatio);
        _maximumX = _minimumX + nextXSpan;
        _minimumY = anchor.Y - (nextYSpan * yRatio);
        _maximumY = _minimumY + nextYSpan;
        InvalidateVisual();
    }

    private static bool SegmentMayIntersectBounds(Point first, Point second, Rect bounds)
    {
        var minX = Math.Min(first.X, second.X);
        var maxX = Math.Max(first.X, second.X);
        var minY = Math.Min(first.Y, second.Y);
        var maxY = Math.Max(first.Y, second.Y);
        return maxX >= 0d && minX <= bounds.Width && maxY >= 0d && minY <= bounds.Height;
    }

    private static string FormatCoordinate(GraphPoint point) =>
        $"x = {point.X:G8}, y = {point.Y:G8}";

    private static void NormalizeRange(ref double minimum, ref double maximum)
    {
        if (!double.IsFinite(minimum) || !double.IsFinite(maximum))
        {
            minimum = -10d;
            maximum = 10d;
            return;
        }

        if (minimum == maximum)
        {
            var expansion = Math.Max(1d, Math.Abs(minimum) * 0.1d);
            minimum -= expansion;
            maximum += expansion;
        }
    }

    private IBrush? Foreground =>
        this.FindResource("SystemControlForegroundBaseHighBrush") as IBrush ?? Brushes.DodgerBlue;
}
