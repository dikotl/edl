namespace Edl.Core.Parsing;

public abstract class Expression
{
    public required LineInfo Location { get; init; }
}

public sealed class Identifier : Expression
{
    public required string Name { get; init; }
}

public sealed class IntLiteral : Expression
{
    public required long Value { get; init; }
}

public sealed class FloatLiteral : Expression
{
    public required double Value { get; init; }
}

public sealed class StringLiteral : Expression
{
    public required string Data { get; init; }
}

public sealed class ExpressionList : Expression
{
    public required Expression[] Items { get; init; }
}

public sealed class Function : Expression
{
    public required Identifier[] Parameters { get; init; }
    public required Expression[] Body { get; init; }
}

public sealed class Call : Expression
{
    public required Identifier Function { get; init; }
    public required Expression[] Arguments { get; init; }
}
