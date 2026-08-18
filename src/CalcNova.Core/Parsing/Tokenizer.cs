using CalcNova.Core.Errors;

namespace CalcNova.Core.Parsing;

public sealed class Tokenizer
{
    private readonly string _input;
    private int _position;

    public Tokenizer(string input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }

    public IReadOnlyList<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _input.Length)
        {
            var current = _input[_position];
            if (char.IsWhiteSpace(current))
            {
                _position++;
                continue;
            }

            if (char.IsDigit(current) || (current == '.' && PeekDigit(1)))
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if (char.IsLetter(current) || current == '_')
            {
                tokens.Add(ReadIdentifier());
                continue;
            }

            var start = _position++;
            tokens.Add(current switch
            {
                '+' => new Token(TokenKind.Plus, "+", start),
                '-' or '−' => new Token(TokenKind.Minus, "-", start),
                '*' or '×' => new Token(TokenKind.Star, "*", start),
                '/' or '÷' => new Token(TokenKind.Slash, "/", start),
                '%' => new Token(TokenKind.Percent, "%", start),
                '^' => new Token(TokenKind.Caret, "^", start),
                '(' => new Token(TokenKind.LeftParenthesis, "(", start),
                ')' => new Token(TokenKind.RightParenthesis, ")", start),
                ',' => new Token(TokenKind.Comma, ",", start),
                _ => throw Syntax($"Unexpected character '{current}'.", start)
            });
        }

        tokens.Add(new Token(TokenKind.End, string.Empty, _input.Length));
        return tokens;
    }

    private Token ReadNumber()
    {
        var start = _position;
        var sawDecimalPoint = false;

        while (_position < _input.Length)
        {
            var current = _input[_position];
            if (char.IsDigit(current))
            {
                _position++;
                continue;
            }

            if (current == '.' && !sawDecimalPoint)
            {
                sawDecimalPoint = true;
                _position++;
                continue;
            }

            break;
        }

        if (_position < _input.Length && (_input[_position] == 'e' || _input[_position] == 'E'))
        {
            var exponentMarker = _position++;
            if (_position < _input.Length && (_input[_position] == '+' || _input[_position] == '-'))
            {
                _position++;
            }

            var exponentStart = _position;
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                _position++;
            }

            if (exponentStart == _position)
            {
                throw Syntax("Scientific notation requires exponent digits.", exponentMarker);
            }
        }

        return new Token(TokenKind.Number, _input[start.._position], start);
    }

    private Token ReadIdentifier()
    {
        var start = _position++;
        while (_position < _input.Length)
        {
            var current = _input[_position];
            if (!char.IsLetterOrDigit(current) && current != '_')
            {
                break;
            }

            _position++;
        }

        return new Token(TokenKind.Identifier, _input[start.._position], start);
    }

    private bool PeekDigit(int offset)
    {
        var index = _position + offset;
        return index < _input.Length && char.IsDigit(_input[index]);
    }

    private static CalculationException Syntax(string message, int position) =>
        new(CalculationErrorCode.SyntaxError, $"{message} Position: {position}.");
}
