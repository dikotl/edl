using Edl.Core.Parsing;

namespace Edl.Core.VirtualMachine;

public abstract class Value(LineInfo origin = new())
{
    public LineInfo Origin { get; init; } = origin;

    public abstract string Display();
}

public sealed class IntValue(long data, LineInfo origin = new()) : Value(origin)
{
    public long Data { get; init; } = data;

    public override string Display() => $"{Data}";
}

public sealed class FloatValue(double data, LineInfo origin = new()) : Value(origin)
{
    public double Data { get; init; } = data;

    public override string Display() => $"{Data}";
}

public sealed class StringValue(string data, LineInfo origin = new()) : Value(origin)
{
    public string Data { get; init; } = data;

    public override string Display() => $"{Data}";
}

public sealed class ListValue(Value[] elements, LineInfo origin = new()) : Value(origin)
{
    public Value[] Elements { get; init; } = elements;

    public override string Display() => "[" + string.Join(" ", Elements.Select(e => e.Display())) + "]";
}

public sealed class ClosureValue : Value
{
    public required string[] Parameters { get; init; }
    public required ICommand[] CompiledBody { get; init; }
    public required Environment CapturedEnv { get; init; }

    public override string Display() => "<closure>";
}

public sealed class IntrinsicValue(IIntrinsic intrinsic, LineInfo origin = new()) : Value(origin)
{
    public IIntrinsic Intrinsic { get; init; } = intrinsic;

    public override string Display() => "<command>";
}
