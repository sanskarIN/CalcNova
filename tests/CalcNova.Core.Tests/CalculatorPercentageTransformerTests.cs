// tests/CalcNova.Core.Tests/CalculatorPercentageTransformerTests.cs
using CalcNova.Core.Evaluation;
using Xunit;

namespace CalcNova.Core.Tests;

public class CalculatorPercentageTransformerTests
{
    [Theory]
    [InlineData("100 + 10%", "(100 + (100 * (10 / 100.0)))")]
    [InlineData("200 - 15%", "(200 - (200 * (15 / 100.0)))")]
    [InlineData("50 * 20%", "(50 * (20 / 100.0))")]
    [InlineData("100 / 25%", "(100 / (25 / 100.0))")]
    [InlineData("50%", "(50 / 100.0)")]
    public void Transform_ProducesExpectedMathStructure(string input, string expected)
    {
        string actual = CalculatorPercentageTransformer.Transform(input);
        Assert.Equal(expected, actual);
    }
}
