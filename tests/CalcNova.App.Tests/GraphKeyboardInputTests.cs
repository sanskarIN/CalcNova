using Avalonia.Input;
using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphKeyboardInputTests
{
    [Theory]
    [InlineData(Key.Left, GraphKeyboardAction.PanLeft)]
    [InlineData(Key.Right, GraphKeyboardAction.PanRight)]
    [InlineData(Key.Up, GraphKeyboardAction.PanUp)]
    [InlineData(Key.Down, GraphKeyboardAction.PanDown)]
    [InlineData(Key.Add, GraphKeyboardAction.ZoomIn)]
    [InlineData(Key.Subtract, GraphKeyboardAction.ZoomOut)]
    [InlineData(Key.Home, GraphKeyboardAction.ResetViewport)]
    [InlineData(Key.F, GraphKeyboardAction.FitToData)]
    public void GetAction_UnmodifiedGraphKey_ReturnsExpectedAction(Key key, GraphKeyboardAction expected)
    {
        Assert.Equal(expected, GraphKeyboardInput.GetAction(key, KeyModifiers.None));
    }

    [Fact]
    public void GetAction_ModifiedKey_DoesNotHijackPlatformShortcut()
    {
        Assert.Equal(
            GraphKeyboardAction.None,
            GraphKeyboardInput.GetAction(Key.Home, KeyModifiers.Control));
    }

    [Fact]
    public void GetAction_UnmappedKey_ReturnsNone()
    {
        Assert.Equal(GraphKeyboardAction.None, GraphKeyboardInput.GetAction(Key.Enter, KeyModifiers.None));
    }
}
