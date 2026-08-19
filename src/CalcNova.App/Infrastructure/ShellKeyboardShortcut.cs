using Avalonia.Input;

namespace CalcNova.App.Infrastructure;

public enum ShellNavigationAction
{
    None,
    PreviousMode,
    NextMode,
    FirstMode,
    LastMode
}

public static class ShellKeyboardShortcut
{
    public static ShellNavigationAction GetNavigationAction(Key key, KeyModifiers modifiers)
    {
        if (modifiers != KeyModifiers.Control)
        {
            return ShellNavigationAction.None;
        }

        return key switch
        {
            Key.PageUp => ShellNavigationAction.PreviousMode,
            Key.PageDown => ShellNavigationAction.NextMode,
            Key.Home => ShellNavigationAction.FirstMode,
            Key.End => ShellNavigationAction.LastMode,
            _ => ShellNavigationAction.None
        };
    }
}
