using System.Diagnostics;
using System.Text;

namespace Edl.Core.Parsing;

public class Lexer(string source, string filename)
{
    static readonly char[] SymbolChars = ['$', '_', '=', '<', '>', '+', '-', '*', '/', '!', '%', '^', '&', '|'];

    private readonly string _source = source;
    private readonly string _filename = filename;

    private uint _pos = 0;
    private uint _line = 1;
    private uint _column = 1;
    private bool _wasStmtSeparator = false;

    private char? Char
        => _pos < _source.Length ? _source[(int)_pos] : null;

    private bool IsEOF
        => _pos >= _source.Length;

    public LineInfo CurrentLineInfo
        => new(_filename, _line, _column);

    public ParseException Error(string message)
        => new(message, _filename, _pos, _line, _column);

    public Token? NextToken()
    {
        SkipWhitespace();
        var start = CurrentLineInfo;
        var wasStmtSeparator = _wasStmtSeparator;
        _wasStmtSeparator = false;

        switch (Char)
        {
        case null:
            return null;

        case '#':
            while (Char is not (null or '\r' or '\n')) Advance();
            return NextToken();

        case '\r':
            Advance();
            if (Char is '\n') Advance();
            if (wasStmtSeparator) return NextToken();
            _wasStmtSeparator = true;
            return new Token(TokenKind.Semicolon, start, "\r\n");

        case '\n':
            Advance();
            if (wasStmtSeparator) return NextToken();
            _wasStmtSeparator = true;
            return new Token(TokenKind.Semicolon, start, "\n");

        case ';':
            Advance();
            if (wasStmtSeparator) return NextToken();
            _wasStmtSeparator = true;
            return new Token(TokenKind.Semicolon, start, ";");

        case '0':
            return ParseNumberFromZero();

        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
            var number = new StringBuilder();
            ParseNumber(number, char.IsAsciiDigit);
            return ParseNumberFromNonZero(number, start);

        case '"':
            return ParseString(CurrentLineInfo);

        case '(':
            Advance();
            return new Token(TokenKind.LeParen, start);

        case ')':
            Advance();
            return new Token(TokenKind.RiParen, start);

        case '[':
            Advance();
            return new Token(TokenKind.LeBracket, start);

        case ']':
            Advance();
            return new Token(TokenKind.RiBracket, start);

        case '{':
            Advance();
            return new Token(TokenKind.LeCurly, start);

        case '}':
            Advance();
            return new Token(TokenKind.RiCurly, start);

        case '\\':
            Advance();
            return new Token(TokenKind.ParamBracket, start);

        default:
            char ch = Char ?? throw new UnreachableException("Null case handled above");

            if (char.IsLetter(ch) || SymbolChars.Contains(ch))
            {
                var name = new StringBuilder();

                while (Char is char c && (char.IsLetterOrDigit(c) || SymbolChars.Contains(c)))
                {
                    name.Append(c);
                    Advance();
                }

                return new Token(TokenKind.Name, start, name.ToString());
            }
            else
            {
                throw Error($"unexpected character '{ch:h}'");
            }
        }
    }

    private Token ParseNumberFromZero()
    {
        var start = CurrentLineInfo;
        var buffer = new StringBuilder();
        Consume('0');

        if (Char is 'x' or 'X')
        {
            buffer.Append("0x");
            ParseNumber(buffer, char.IsAsciiHexDigit);
            return new Token(TokenKind.Int, start, buffer.ToString());
        }
        else if (Char is 'b' or 'B')
        {
            buffer.Append("0b");
            ParseNumber(buffer, c => c is '0' or '1');
            return new Token(TokenKind.Int, start, buffer.ToString());
        }
        else
        {
            buffer.Append('0');
        }

        while (Char is char ch && char.IsAsciiDigit(ch))
        {
            Advance();
            buffer.Append(ch);
        }

        if (Char is char c && char.IsLetter(c))
        {
            throw Error("unexpected letter after digit, identifier cannot start with digit");
        }

        return ParseNumberFromNonZero(buffer, start);
    }

    private Token ParseNumberFromNonZero(StringBuilder buffer, LineInfo start)
    {
        var kind = TokenKind.Int;

        if (Char is '.')
        {
            buffer.Append('.');
            Advance();
            kind = TokenKind.Float;
            ParseNumber(buffer, char.IsAsciiDigit);
        }

        if (Char is 'e' or 'E')
        {
            buffer.Append(Char);
            Advance();
            kind = TokenKind.Float;

            if (Char is '+' or '-')
            {
                buffer.Append(Char);
                Advance();
            }

            ParseNumber(buffer, char.IsAsciiDigit);
        }

        if (Char is char c && char.IsLetter(c))
        {
            throw Error("unexpected letter after digit, identifier cannot start with digit");
        }

        return new Token(kind, start, buffer.ToString());
    }

    private void ParseNumber(StringBuilder buffer, Predicate<char> predicate)
    {
        var written = 0;
        var wasUnderscore = false;

        while (true)
        {
            if (Char is null || (Char is char c && !predicate(c)))
            {
                if (wasUnderscore)
                    throw Error("unexpected character after underscore");

                if (written == 0)
                    throw Error("no digits was parsed");

                return;
            }

            buffer.Append(Char);
            Advance();
            ++written;

            if (Char == '_')
            {
                Advance();
                wasUnderscore = true;
            }
        }
    }

    private Token ParseString(LineInfo start)
    {
        var buffer = new StringBuilder();
        Consume('"');

        while (true)
        {
            switch (Char)
            {
            case null:
            case '\n':
            case '\r':
                throw Error("string not closed");

            case '"':
                Advance();
                return new Token(TokenKind.String, start, buffer.ToString());

            case '\\':
                Advance();

                char unescaped = Char switch
                {
                    '\\' => '\\',
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '"' => '"',
                    char c => throw Error($"unexpected escape sequence '\\{c}'"),
                    null => throw Error("unexpected end of file in string escape sequence"),
                };

                buffer.Append(unescaped);
                Advance();
                break;

            default:
                buffer.Append(Char);
                Advance();
                break;
            }
        }
    }

    /// <summary>
    /// Parses a character after the backslash (\).
    /// </summary>
    /// <returns>
    /// Replacement character, or null if escaped character is not allowed.
    /// </returns>
    private char? ParseEscapeSequence() => Advance() switch
    {
        'n' => '\n',
        'r' => '\r',
        't' => '\t',
        '\\' => '\\',
        '\'' => '\'',
        '"' => '"',
        _ => null
    };

    /// <summary>
    /// Heuristic to determine if ' is starting a char literal ('x')
    /// or a quoted symbol ('foo)
    /// </summary>
    private bool IsCharLiteral() =>
        // '\n' (length 4 from start).
        Peek(1) == '\\' && Peek(3) == '\'' ||
        // 'a' (length 3 from start).
        Peek(2) == '\'';

    private void SkipWhitespace()
    {
        while (!IsEOF)
        {
            char c = Peek();

            if (char.IsWhiteSpace(c) && c != '\r' && c != '\n')
            {
                Advance();
            }
            else if (c == '/' && Peek(1) == '/') // Line comment
            {
                Advance(); Advance(); // Eat //
                while (!IsEOF && Peek() is not ('\n' or '\r'))
                {
                    Advance();
                }
            }
            else
            {
                break;
            }
        }
    }

    // --- Cursor Management ---

    private char Peek(int offset = 0)
    {
        if (_pos + offset >= _source.Length)
        {
            return '\0';
        }
        return _source[(int)_pos + offset];
    }

    private char Advance()
    {
        if (IsEOF)
        {
            return '\0';
        }

        char c = _source[(int)_pos++];

        // Update Line/Column info
        if (c == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return c;
    }

    private void Consume(char expected)
    {
        if (IsEOF || _source[(int)_pos] != expected)
        {
            throw Error($"expected {expected}");
        }
        Advance();
    }
}
