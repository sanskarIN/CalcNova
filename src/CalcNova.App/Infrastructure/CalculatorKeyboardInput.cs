using Avalonia.Input;

namespace CalcNova.App.Infrastructure;

public static class CalculatorKeyboardInput
{
    public static bool TryGetToken(Key key, out string token)
    {
        token = key switch
        {
            Key.D0 or Key.NumPad0 => "0",
            Key.D1 or Key.NumPad1 => "1",
            Key.D2 or Key.NumPad2 => "2",
            Key.D3 or Key.NumPad3 => "3",
            Key.D4 or Key.NumPad4 => "4",
            Key.D5 or Key.NumPad5 => "5",
            Key.D6 or Key.NumPad6 => "6",
            Key.D7 or Key.NumPad7 => "7",
            Key.D8 or Key.NumPad8 => "8",
            Key.D9 or Key.NumPad9 => "9",
            Key.Add => "+",
            Key.Subtract or Key.OemMinus => "-",
            Key.Multiply => "*",
            Key.Divide or Key.OemQuestion => "/",
            Key.Decimal or Key.OemPeriod => ".",
            Key.OemComma => ",",
            _ => string.Empty
        };

        return token.Length > 0;
    }

    public static bool TryGetModifiedToken(Key key, KeyModifiers modifiers, out string token)
    {
        if (modifiers != KeyModifiers.Shift)
        {
            token = string.Empty;
            return false;
        }

        token = key switch
        {
            Key.OemPlus => "+",
            Key.D8 => "*",
            Key.D9 => "(",
            Key.D0 => ")",
            Key.D6 => "^",
            Key.D5 => "%",
            _ => string.Empty
        };

        return token.Length > 0;
    }
}
