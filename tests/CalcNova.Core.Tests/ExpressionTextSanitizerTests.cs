using CalcNova.Core.Parsing;
using Xunit;

namespace CalcNova.Core.Tests;

public sealed class ExpressionTextSanitizerTests
{
    [Fact]
    public void Sanitize_NormalizesCommonCalculatorGlyphs()
    {
        var sanitized = ExpressionTextSanitizer.Sanitize(" = 2 × π − 3 ÷ τ ");

        Assert.Equal("2 * pi - 3 / tau", sanitized);
    }

    [Fact]
    public void Sanitize_NormalizesSuperscriptPowers()
    {
        Assert.Equal("12^2 + 3^3", ExpressionTextSanitizer.Sanitize("12² + 3³"));
    }

    [Fact]
    public void Sanitize_ReplacesMultilineWhitespaceWithSafeSpacing()
    {
        Assert.Equal("1 + 2 * 3", ExpressionTextSanitizer.Sanitize("1 +\n2 *\t3"));
    }

    [Fact]
    public void Sanitize_RejectsUnsupportedCharacters()
    {
        var exception = Assert.Throws<ArgumentException>(() => ExpressionTextSanitizer.Sanitize("2 @ 3"));

        Assert.Contains("unsupported character", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Sanitize_RejectsExpressionsPastConfiguredLimit()
    {
        Assert.Throws<ArgumentException>(() => ExpressionTextSanitizer.Sanitize("12345", 4));
    }

    [Fact]
    public void Sanitize_EmptyInputProducesEmptyExpression()
    {
        Assert.Equal(string.Empty, ExpressionTextSanitizer.Sanitize("  \r\n  "));
    }
}
