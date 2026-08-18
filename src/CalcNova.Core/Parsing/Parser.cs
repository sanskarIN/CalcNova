using CalcNova.Core.Errors;

namespace CalcNova.Core.Parsing;

public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private int _index;

    public Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        if (_tokens.Count == 0)
        {
            throw new ArgumentException("Token collection must include an end token.", nameof(tokens));
        }
    }

    public Expression Parse()
    {
        var expression = ParseAdditive();
        if (Current.Kind != TokenKind.End)
        {
            throw Syntax($"Unexpected token '{Current.Text}'.", Current.Position);
        }

        return expression;
    }

    private Expression ParseAdditive()
    {
        var left = ParseMultiplicative();
        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
        {
            var operation = Advance();
            var right = ParseMultiplicative();
            left = new BinaryExpression(left, operation.Kind, right);
        }

        return left;
    }

    private Expression ParseMultiplicative()
    {
        var left = ParseUnary();
        while (Current.Kind is TokenKind.Star or TokenKind.Slash or TokenKind.Percent)
        {
            var operation = Advance();
            var right = ParseUnary();
            left = new BinaryExpression(left, operation.Kind, right);
        }

        return left;
    }

    private Expression ParseUnary()
    {
        if (Current.Kind is TokenKind.Plus or TokenKind.Minus)
        {
            var operation = Advance();
            return new UnaryExpression(operation.Kind, ParseUnary());
        }

        return ParsePower();
    }

    private Expression ParsePower()
    {
        var left = ParsePrimary();
        if (Current.Kind == TokenKind.Caret)
        {
            var operation = Advance();
            var right = ParseUnary();
            return new BinaryExpression(left, operation.Kind, right);
        }

        return left;
    }

    private Expression ParsePrimary()
    {
        if (Current.Kind == TokenKind.Number)
        {
            return new NumberExpression(Advance().Text);
        }

        if (Current.Kind == TokenKind.Identifier)
        {
            var identifier = Advance();
            if (Current.Kind != TokenKind.LeftParenthesis)
            {
                return new ConstantExpression(identifier.Text);
            }

            Advance();
            var arguments = new List<Expression>();
            if (Current.Kind != TokenKind.RightParenthesis)
            {
                do
                {
                    arguments.Add(ParseAdditive());
                }
                while (Match(TokenKind.Comma));
            }

            Consume(TokenKind.RightParenthesis, "Expected ')' after function arguments.");
            return new CallExpression(identifier.Text, arguments);
        }

        if (Match(TokenKind.LeftParenthesis))
        {
            var expression = ParseAdditive();
            Consume(TokenKind.RightParenthesis, "Expected ')' after expression.");
            return expression;
        }

        throw Syntax("Expected a number, constant, function, or parenthesized expression.", Current.Position);
    }

    private Token Current => _tokens[Math.Min(_index, _tokens.Count - 1)];

    private Token Advance()
    {
        var token = Current;
        if (_index < _tokens.Count - 1)
        {
            _index++;
        }

        return token;
    }

    private bool Match(TokenKind kind)
    {
        if (Current.Kind != kind)
        {
            return false;
        }

        Advance();
        return true;
    }

    private Token Consume(TokenKind kind, string message)
    {
        if (Current.Kind == kind)
        {
            return Advance();
        }

        throw Syntax(message, Current.Position);
    }

    private static CalculationException Syntax(string message, int position) =>
        new(CalculationErrorCode.SyntaxError, $"{message} Position: {position}.");
}
