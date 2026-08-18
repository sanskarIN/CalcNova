using System.Numerics;
using CalcNova.Core.Memory;
using CalcNova.Core.Numerics;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class CalculatorMemoryTests
{
    [Fact]
    public void StoreAndRecall_PreserveValue()
    {
        var memory = new CalculatorMemory();
        var value = NumberValue.FromDecimal(12.5m);

        memory.Store(value);

        Assert.True(memory.HasValue);
        Assert.Equal(value, memory.Recall());
    }

    [Fact]
    public void AddAndSubtract_WorkFromEmptyMemory()
    {
        var memory = new CalculatorMemory();

        memory.Add(NumberValue.FromInteger(new BigInteger(10)));
        memory.Subtract(NumberValue.FromInteger(new BigInteger(3)));

        Assert.True(memory.HasValue);
        Assert.Equal("7", memory.Recall().ToDisplayString());
    }

    [Fact]
    public void Clear_ResetsState()
    {
        var memory = new CalculatorMemory();
        memory.Store(NumberValue.FromInteger(new BigInteger(99)));

        memory.Clear();

        Assert.False(memory.HasValue);
        Assert.Equal(NumberValue.Zero, memory.Recall());
    }
}
