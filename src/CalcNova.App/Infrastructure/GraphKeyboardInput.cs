using Avalonia.Input;

namespace CalcNova.App.Infrastructure;

public enum GraphKeyboardAction
{
    None,
    PanLeft,
    PanRight,
    PanUp,
    PanDown,
    ZoomIn,
    ZoomOut,
    ResetViewport,
    FitToData
}

public static class GraphKeyboardInput
{
    public static GraphKeyboardAction GetAction(Key key, KeyModifiers modifiers)
    {
        if (modifiers != KeyModifiers.None)
        {
            return GraphKeyboardAction.None;
        }

        return key switch
        {
            Key.Left => GraphKeyboardAction.PanLeft,
            Key.Right => GraphKeyboardAction.PanRight,
            Key.Up => GraphKeyboardAction.PanUp,
            Key.Down => GraphKeyboardAction.PanDown,
            Key.Add => GraphKeyboardAction.ZoomIn,
            Key.Subtract => GraphKeyboardAction.ZoomOut,
            Key.Home => GraphKeyboardAction.ResetViewport,
            Key.F => GraphKeyboardAction.FitToData,
            _ => GraphKeyboardAction.None
        };
    }
}
