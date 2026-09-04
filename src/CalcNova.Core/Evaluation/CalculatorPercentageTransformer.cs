// src/CalcNova.Core/Evaluation/CalculatorPercentageTransformer.cs
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CalcNova.Core.Evaluation;

/// <summary>
/// Transforms human calculator percentage syntax into mathematically sound expressions
/// prior to AST tokenization and parsing.
/// </summary>
public static class CalculatorPercentageTransformer
{
    // Regex matching: [Left Operand] [Operator (+, -, *, /)] [Percentage Operand]%
    // Example matches: "100 + 10%", "250.5 - 5%", "80 * 20%", "40 / 10%"
    private static readonly Regex BinaryPercentageRegex = new(
        @"(?<left>(?:\d+(?:\.\d+)?|\([^\(\)]+\)))\s*(?<op>[\+\-\*\/])\s*(?<percent>\d+(?:\.\d+)?)\s*%",
        RegexOptions.Compiled);

    // Regex matching standalone percentages: "50%" -> "(50 / 100)"
    private static readonly Regex StandalonePercentageRegex = new(
        @"(?<val>\d+(?:\.\d+)?)\s*%",
        RegexOptions.Compiled);

    /// <summary>
    /// Transforms percentage expressions to respect commercial calculator rules.
    /// Additive/Subtractive: A + B% => A + (A * (B / 100))
    /// Multiplicative/Divisive: A * B% => A * (B / 100)
    /// Standalone: B% => (B / 100)
    /// </summary>
    public static string Transform(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression) || !expression.Contains('%'))
            return expression;

        string current = expression;
        bool modified;

        // Iterate to resolve nested or chained occurrences: e.g., "100 + 10% + 5%"
        do
        {
            modified = false;
            string transformed = BinaryPercentageRegex.Replace(current, match =>
            {
                modified = true;
                string left = match.Groups["left"].Value.Trim();
                string op = match.Groups["op"].Value.Trim();
                string percent = match.Groups["percent"].Value.Trim();

                return op switch
                {
                    "+" => $"({left} + ({left} * ({percent} / 100.0)))",
                    "-" => $"({left} - ({left} * ({percent} / 100.0)))",
                    "*" => $"({left} * ({percent} / 100.0))",
                    "/" => $"({left} / ({percent} / 100.0))",
                    _ => match.Value
                };
            });

            current = transformed;
        } while (modified);

        // Transform any remaining standalone percentages: "(50%)" => "((50 / 100.0))"
        current = StandalonePercentageRegex.Replace(current, match =>
        {
            string val = match.Groups["val"].Value.Trim();
            return $"({val} / 100.0)";
        });

        return current;
    }
}
