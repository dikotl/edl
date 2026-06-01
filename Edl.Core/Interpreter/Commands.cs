using Edl.Core.Parsing;

namespace Edl.Core.Interpreter;

public interface ICommand
{
    void Execute(Context context);
}

public class PushCommand(Value value) : ICommand
{
    public Value Value { get; init; } = value;

    public void Execute(Context context)
    {
        context.Push(Value);
    }

    public override string ToString() => $"push {Value.Display()}";
}

public class PopCommand : ICommand
{
    public void Execute(Context context)
    {
        _ = context.Pop();
    }

    public override string ToString() => "pop";
}

public class ReturnCommand : ICommand
{
    public void Execute(Context context)
    {
        throw new ReturnException();
    }

    public override string ToString() => "return";
}

public class LoadCommand(string variableName) : ICommand
{
    public string VariableName { get; init; } = variableName;

    public void Execute(Context context)
    {
        context.Push(context.CurrentEnvironment.Load(VariableName));
    }

    public override string ToString() => $"load {VariableName}";
}

public class LoadSelfCommand : ICommand
{
    public void Execute(Context context)
    {
        if (context.CurrentClosure is null)
        {
            throw new Exception("cannot load self, not in closure");
        }
        context.Push(context.CurrentClosure);
    }

    public override string ToString() => $"load-self";
}

public class StoreCommand(string variableName) : ICommand
{
    public string VariableName { get; init; } = variableName;

    public void Execute(Context context)
    {
        context.CurrentEnvironment.Store(VariableName, context.Peek());
    }

    public override string ToString() => $"store {VariableName}";
}

public class CallCommand(int argCount) : ICommand
{
    public int ArgCount { get; init; } = argCount;

    public void Execute(Context context)
    {
        context.Call(ArgCount);
    }

    public override string ToString() => $"call {ArgCount}";
}

public class MakeClosureCommand(string[] parameters, string[] captures, ICommand[] body, LineInfo origin) : ICommand
{
    public string[] Parameters { get; init; } = parameters;
    public string[] Captures { get; init; } = captures;
    public ICommand[] CompiledBody { get; init; } = body;
    public LineInfo Origin { get; init; } = origin;

    public void Execute(Context context)
    {
        // TODO: fix captures not include all all variables.
        var env = context.CurrentEnvironment.Clone();
        foreach (var capture in Captures)
        {
            env.Store(capture, context.CurrentEnvironment.Load(capture));
        }
        var closure = new ClosureValue
        {
            Parameters = Parameters,
            CompiledBody = CompiledBody,
            CapturedEnv = env,
            Origin = Origin,
        };
        context.Push(closure);
    }

    private int indentCount = 0;

    public override string ToString()
    {
        var parameters = string.Join(", ", Parameters);
        var indent = new string(' ', indentCount += 2);
        var body = string.Join($"\n{indent}", CompiledBody.Select(cmd => cmd.ToString()));
        indentCount -= 2;
        return $"make-closure ({parameters})\n{indent}{body}";
    }
}

public class MakeListCommand(int elementCount, LineInfo origin) : ICommand
{
    public int ElementCount { get; init; } = elementCount;
    public LineInfo Origin { get; init; } = origin;

    public void Execute(Context context)
    {
        var elements = new Value[ElementCount];
        for (int i = ElementCount - 1; i >= 0; i--)
        {
            elements[i] = context.Pop();
        }
        context.Push(new ListValue(elements, Origin));
    }

    public override string ToString() => $"make-list {ElementCount}";
}

public class IfCommand(ICommand[] trueBranch, ICommand[] falseBranch) : ICommand
{
    public ICommand[] TrueBranch { get; init; } = trueBranch;
    public ICommand[] FalseBranch { get; init; } = falseBranch;

    public void Execute(Context context)
    {
        var condition = ((IntValue)context.Pop()).Data;
        if (condition != 0)
        {
            context.Execute(TrueBranch);
        }
        else
        {
            context.Execute(FalseBranch);
        }
    }
    private int indentCount = 0;

    public override string ToString()
    {
        var indent = new string(' ', indentCount += 2);
        var thenBody = string.Join($"\n{indent}", TrueBranch.Select(cmd => cmd.ToString()));
        var elseBody = string.Join($"\n{indent}", FalseBranch.Select(cmd => cmd.ToString()));
        indentCount -= 2;

        return $"branch\nthen:\n{indent}{thenBody}\n{indent}{elseBody}";
    }
}

public class ForeachCommand : ICommand
{
    public void Execute(Context context)
    {
        var iterable = context.Pop();
        var body = context.Pop();

        switch (iterable)
        {
        case StringValue str:
            foreach (var chr in str.Data)
            {
                context.Push(body);
                context.Push(new IntValue(chr));
                context.Call(1);
            }
            break;

        case ListValue list:
            foreach (var element in list.Elements)
            {
                context.Push(body);
                context.Push(element);
                context.Call(1);
            }
            break;

        default:
            throw new Exception($"Cannot iterate over {iterable.GetType().Name} value");
        }
    }
}
