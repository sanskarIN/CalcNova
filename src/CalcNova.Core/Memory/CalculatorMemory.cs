using CalcNova.Core.Numerics;

namespace CalcNova.Core.Memory;

public sealed class CalculatorMemory
{
    private NumberValue _value = NumberValue.Zero;

    public bool HasValue { get; private set; }

    public NumberValue Recall() => _value;

    public void Clear()
    {
        _value = NumberValue.Zero;
        HasValue = false;
    }

    public void Store(NumberValue value)
    {
        _value = value;
        HasValue = true;
    }

    public void Add(NumberValue value)
    {
        _value = (HasValue ? _value : NumberValue.Zero).Add(value);
        HasValue = true;
    }

    public void Subtract(NumberValue value)
    {
        _value = (HasValue ? _value : NumberValue.Zero).Subtract(value);
        HasValue = true;
    }
}
