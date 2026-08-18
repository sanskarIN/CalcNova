namespace CalcNova.Core.Parsing;

public abstract record Expression;

public sealed record NumberExpression(string Literal) : Expression;

public sealed record ConstantExpression(string Name) : Expression;

public sealed record UnaryExpression(TokenKind Operator, Expression Operand) : Expression;

public sealed record BinaryExpression(Expression Left, TokenKind Operator, Expression Right) : Expression;

public sealed record CallExpression(string Name, IReadOnlyList<Expression> Arguments) : Expression;
