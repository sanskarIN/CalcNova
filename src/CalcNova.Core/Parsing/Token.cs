namespace CalcNova.Core.Parsing;

public readonly record struct Token(TokenKind Kind, string Text, int Position);
