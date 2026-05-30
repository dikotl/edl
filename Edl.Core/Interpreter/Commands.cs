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
}

public class PopCommand : ICommand
{
    public void Execute(Context context)
    {
        _ = context.Pop();
    }
}

public class ReturnCommand : ICommand
{
    public void Execute(Context context)
    {
        throw new ReturnException();
    }
}

public class LoadCommand(string variableName) : ICommand
{
    public string VariableName { get; init; } = variableName;

    public void Execute(Context context)
    {
        context.Push(context.CurrentEnvironment.Load(VariableName));
    }
}

public class StoreCommand(string variableName) : ICommand
{
    public string VariableName { get; init; } = variableName;

    public void Execute(Context context)
    {
        context.CurrentEnvironment.Store(VariableName, context.Peek());
    }
}

public class CallCommand(string functionName, int argCount) : ICommand
{
    public string FunctionName { get; init; } = functionName;
    public int ArgCount { get; init; } = argCount;

    public void Execute(Context context)
    {
        context.Call(FunctionName, ArgCount);
    }
}

public class MakeClosureCommand(string[] parameters, ICommand[] body, LineInfo origin) : ICommand
{
    public string[] Parameters { get; init; } = parameters;
    public ICommand[] CompiledBody { get; init; } = body;
    public LineInfo Origin { get; init; } = origin;

    public void Execute(Context context)
    {
        var closure = new ClosureValue
        {
            Parameters = Parameters,
            CompiledBody = CompiledBody,
            CapturedEnv = context.CurrentEnvironment.Clone(),
            Origin = Origin,
        };
        context.Push(closure);
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
}
