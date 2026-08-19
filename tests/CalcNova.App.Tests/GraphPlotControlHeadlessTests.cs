using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using CalcNova.App.Controls;
using CalcNova.Graphing;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphPlotControlHeadlessTests
{
    [AvaloniaFact]
    public void KeyboardPanAndZoom_UpdateViewport()
    {
        var plot = new GraphPlotControl();
        var window = new Window { Width = 600, Height = 400, Content = plot };

        window.Show();
        try
        {
            Assert.True(plot.Focus());
            Assert.Equal(new GraphViewport(-10d, 10d, -10d, 10d), plot.Viewport);

            window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
            Assert.Equal(-8d, plot.Viewport.MinimumX, precision: 10);
            Assert.Equal(12d, plot.Viewport.MaximumX, precision: 10);

            window.KeyPressQwerty(PhysicalKey.NumpadAdd, RawInputModifiers.None);
            Assert.Equal(16.4d, plot.Viewport.Width, precision: 10);
            Assert.Equal(16.4d, plot.Viewport.Height, precision: 10);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void HomeKey_ResetsViewportAfterKeyboardNavigation()
    {
        var plot = new GraphPlotControl();
        var window = new Window { Width = 600, Height = 400, Content = plot };

        window.Show();
        try
        {
            Assert.True(plot.Focus());
            window.KeyPressQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.None);
            window.KeyPressQwerty(PhysicalKey.NumpadSubtract, RawInputModifiers.None);
            Assert.NotEqual(new GraphViewport(-10d, 10d, -10d, 10d), plot.Viewport);

            window.KeyPressQwerty(PhysicalKey.Home, RawInputModifiers.None);

            Assert.Equal(new GraphViewport(-10d, 10d, -10d, 10d), plot.Viewport);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void FitKey_FitsFiniteSampledData()
    {
        var plot = new GraphPlotControl
        {
            Segments =
            [
                new GraphSegment(
                    [
                        new GraphPoint(-2d, -4d),
                        new GraphPoint(0d, 0d),
                        new GraphPoint(3d, 9d)
                    ])
            ]
        };
        var window = new Window { Width = 600, Height = 400, Content = plot };

        window.Show();
        try
        {
            Assert.True(plot.Focus());
            plot.ResetViewport();
            Assert.Equal(new GraphViewport(-10d, 10d, -10d, 10d), plot.Viewport);

            window.KeyPressQwerty(PhysicalKey.F, RawInputModifiers.None);

            Assert.True(plot.Viewport.MinimumX < -2d);
            Assert.True(plot.Viewport.MaximumX > 3d);
            Assert.True(plot.Viewport.MinimumY < -4d);
            Assert.True(plot.Viewport.MaximumY > 9d);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void MultiSeries_FitsCombinedFiniteData()
    {
        var plot = new GraphPlotControl
        {
            Series =
            [
                new GraphExpressionSample(
                    new GraphExpressionDefinition("series-1", "f1", "x"),
                    [new GraphSegment([new GraphPoint(-5d, -1d), new GraphPoint(0d, 0d)])],
                    0),
                new GraphExpressionSample(
                    new GraphExpressionDefinition("series-2", "f2", "x ^ 2"),
                    [new GraphSegment([new GraphPoint(0d, 0d), new GraphPoint(4d, 16d)])],
                    0)
            ]
        };
        var window = new Window { Width = 600, Height = 400, Content = plot };

        window.Show();
        try
        {
            plot.FitToData();

            Assert.True(plot.Viewport.MinimumX < -5d);
            Assert.True(plot.Viewport.MaximumX > 4d);
            Assert.True(plot.Viewport.MinimumY < -1d);
            Assert.True(plot.Viewport.MaximumY > 16d);
        }
        finally
        {
            window.Close();
        }
    }
}
