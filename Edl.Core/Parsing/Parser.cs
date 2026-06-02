using System.Diagnostics;

namespace Edl.Core.Parsing;

public class Parser(Lexer lexer)
{
    public Parser(string source, string filename) : this(new Lexer(source, filename))
    {
    }

    private readonly Lexer _lexer = lexer;
    private Token? _token = lexer.NextToken();
    private List<ParseException> _errors = [];

    private TokenKind Kind => _token?.Kind ?? throw _lexer.Error("unexpected end of file");

    private bool IsEOF => _token is null;

    private Token? Next()
    {
        var old = _token;
        _token = _lexer.NextToken();
        return old;
    }

    private ParseException Error(string message) => _lexer.Error(message);

    private Token Expect(TokenKind kind, string? expected = null)
    {
        if (_token is Token t && t.Kind == kind)
        {
            Next();
            return t;
        }
        throw Error($"unexpected {(_token?.Kind).ToPrettyString()}, expected {expected ?? kind.ToPrettyString()}");
    }

    private Expression ParseExpression()
    {
        switch (_token?.Kind)
        {
        case TokenKind.Int:
        {
            var token = Expect(TokenKind.Int);
            var value = long.Parse(token.Data);
            return new IntLiteral
            {
                Value = value,
                Location = token.LineInfo,
            };
        }

        case TokenKind.Float:
        {
            var token = Expect(TokenKind.Float);
            var value = double.Parse(token.Data);
            return new FloatLiteral
            {
                Value = value,
                Location = token.LineInfo,
            };
        }

        case TokenKind.String:
        {
            var token = Expect(TokenKind.String);
            return new StringLiteral
            {
                Data = token.Data,
                Location = token.LineInfo,
            };
        }

        case TokenKind.Name:
        {
            return ParseName();
        }

        case TokenKind.LeParen:
        {
            var token = Expect(TokenKind.LeParen);
            var name = ParseName();
            var args = ParseSequenceUntilDelimiter(ParseExpression, [TokenKind.RiParen]).ToArray();
            Expect(TokenKind.RiParen);
            return new Call
            {
                Function = name,
                Arguments = args,
                Location = token.LineInfo,
            };
        }

        case TokenKind.LeCurly:
        {
            var token = Expect(TokenKind.LeCurly);
            var items = ParseSequenceUntilDelimiter(() => ParseStatement(true), [TokenKind.RiCurly], TokenKind.Semicolon).ToArray();
            Expect(TokenKind.RiCurly);
            return new Function
            {
                Parameters = [],
                Body = items,
                Location = token.LineInfo,
            };
        }

        case TokenKind.LeBracket:
        {
            var token = Expect(TokenKind.LeBracket);
            var items = ParseSequenceUntilDelimiter(ParseExpression, [TokenKind.RiBracket]).ToArray();
            Expect(TokenKind.RiBracket);
            return new ExpressionList
            {
                Items = items,
                Location = token.LineInfo,
            };
        }

        case TokenKind.RiParen:
        case TokenKind.RiCurly:
        case TokenKind.RiBracket:
            throw Error("unexpected closing paren");

        case TokenKind.ParamBracket:
        {
            var token = Expect(TokenKind.ParamBracket);
            var parameters = ParseSequenceUntilDelimiter(ParseName, [TokenKind.ParamBracket]).ToArray();
            Expect(TokenKind.ParamBracket);
            var bodyStart = Expect(TokenKind.LeCurly);
            var body = ParseSequenceUntilDelimiter(() => ParseStatement(true), [TokenKind.RiCurly], TokenKind.Semicolon).ToArray();
            Expect(TokenKind.RiCurly);
            return new Function
            {
                Parameters = parameters,
                Body = body,
                Location = token.LineInfo,
            };
        }

        case TokenKind.Semicolon:
            throw Error("unexpected semicolon or newline");

        case null:
            throw Error("unexpected end of file");

        default:
            throw new UnreachableException($"unhandled token kind '{_token?.Kind}'");
        }
    }

    private Identifier ParseName()
    {
        var token = Expect(TokenKind.Name);
        return new Identifier
        {
            Name = token.Data,
            Location = token.LineInfo,
        };
    }

    private Expression? ParseStatement(bool inCurly)
    {
        while (_token?.Kind is TokenKind.Semicolon)
        {
            Next();
        }

        if (IsEOF || (inCurly && Kind == TokenKind.RiCurly))
        {
            return null;
        }

        var expression = ParseExpression();

        if (expression is Identifier name)
        {
            TokenKind[] delimiters = inCurly
                                   ? [TokenKind.Semicolon, TokenKind.RiCurly]
                                   : [TokenKind.Semicolon];
            var args = ParseSequenceUntilDelimiter(ParseExpression, delimiters).ToArray();
            return new Call
            {
                Function = name,
                Arguments = args,
                Location = name.Location
            };
        }
        else
        {
            return expression;
        }
    }

    public IEnumerable<(Expression?, ParseException[])> Parse()
    {
        while (true)
        {
            while (_token?.Kind is TokenKind.Semicolon)
            {
                Next();
            }

            if (IsEOF)
            {
                break;
            }

            var node = ParseStatement(false);
            var errors = _errors.ToArray();
            _errors.Clear();
            yield return (node, errors);
        }
    }

    private IEnumerable<T> ParseSequenceUntilDelimiter<T>(
        Func<T?> parser,
        TokenKind[] delimiters,
        TokenKind? separator = null
    )
        where T : class
    {
        while (!IsEOF && !delimiters.Contains(Kind))
        {
            ParseException? error = null;
            T? node = null;

            try
            {
                node = parser.Invoke();
            }
            catch (ParseException e)
            {
                error = e;
            }

            if (error is null)
            {
                bool separatorSkipped = separator is null;

                if (separator is not null && Kind == separator)
                {
                    Next();
                    separatorSkipped = true;
                }
                if (separatorSkipped || delimiters.Contains(Kind))
                {
                    if (node is not null)
                    {
                        yield return node;
                    }
                    continue;
                }

                error = Error(separator is null
                    ? $"expected {string.Join(" or ", delimiters.Select(x => x.ToPrettyString()))}, found {_token?.Kind.ToPrettyString()}"
                    : $"expected {string.Join(", ", delimiters.Select(x => x.ToPrettyString()))} or {separator.ToPrettyString()}, found {_token?.Kind.ToPrettyString()}"
                );
            }

            while (!IsEOF && !delimiters.Contains(Kind))
            {
                Next();
            }

            _errors.Add(error);
        }
    }
}
