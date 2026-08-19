using Avalonia.Input;
using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ShellKeyboardShortcutTests
{
    [Theory]
    [InlineData(Key.PageUp, ShellNavigationAction.PreviousMode)]
    [InlineData(Key.PageDown, ShellNavigationAction.NextMode)]
    [InlineData(Key.Home, ShellNavigationAction.FirstMode)]
    [InlineData(Key.End, ShellNavigationAction.LastMode)]
    public void GetNavigationAction_ControlShortcut_ReturnsExpectedAction(
        Key key,
        ShellNavigationAction expected)
    {
        var action = ShellKeyboardShortcut.GetNavigationAction(key, KeyModifiers.Control);

        Assert.Equal(expected, action);
    }

    [Fact]
    public void GetNavigationAction_WithoutControl_DoesNotNavigate()
    {
        var action = ShellKeyboardShortcut.GetNavigationAction(Key.PageDown, KeyModifiers.None);

        Assert.Equal(ShellNavigationAction.None, action);
    }

    [Fact]
    public void GetNavigationAction_WithAdditionalModifier_DoesNotNavigate()
    {
        var action = ShellKeyboardShortcut.GetNavigationAction(
            Key.PageDown,
            KeyModifiers.Control | KeyModifiers.Shift);

        Assert.Equal(ShellNavigationAction.None, action);
    }
}
