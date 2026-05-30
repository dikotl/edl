using System.Diagnostics;

namespace Edl.Core.Parsing;

public enum TokenKind
{
    Int,
    Float,
    String,
    Name,

    LeParen,
    RiParen,
    LeCurly,
    RiCurly,
    LeBracket,
    RiBracket,
    ParamBracket,

    Semicolon,
}

static class TokenKindExt
{
    public static string ToPrettyString(this TokenKind kind) => kind switch
    {
        TokenKind.Int => "int literal",
        TokenKind.Float => "float literal",
        TokenKind.String => "string literal",
        TokenKind.Name => "identifier",
        TokenKind.LeParen => "'('",
        TokenKind.RiParen => "')'",
        TokenKind.LeCurly => "'{'",
        TokenKind.RiCurly => "'}'",
        TokenKind.LeBracket => "'['",
        TokenKind.RiBracket => "']'",
        TokenKind.ParamBracket => "'\\'",
        TokenKind.Semicolon => "';'",
        _ => throw new UnreachableException(),
    };

    public static string ToPrettyString(this TokenKind? kind) => kind switch
    {
        null => "end of file",
        TokenKind x => x.ToPrettyString(),
    };
}
