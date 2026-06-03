namespace Edl.Core.Parsing;

public readonly record struct Token(TokenKind Kind, LineInfo LineInfo, string Data = "");
