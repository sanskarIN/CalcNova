using CalcNova.Core.Parsing;

namespace CalcNova.Core.Evaluation;

public sealed class CompiledExpression
{
    internal CompiledExpression(string source, Expression syntaxTree)
    {
        Source = source;
        SyntaxTree = syntaxTree;
    }

    public string Source { get; }

    internal Expression SyntaxTree { get; }
}
