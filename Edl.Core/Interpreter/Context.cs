namespace Edl.Core.Interpreter;

public sealed class Context
{
    private static readonly Dictionary<string, ICommand> INTRINSICS = Registry.Load<ICommand, IntrinsicAttribute>();
    private readonly Stack<Value> _stack = new();

    public Environment CurrentEnvironment { get; set; } = new();

    public Context()
    {
        foreach (var (intrinsicName, command) in INTRINSICS)
        {
            CurrentEnvironment.Store(intrinsicName, new CommandValue(command));
        }
    }

    public void Push(Value value)
    {
        _stack.Push(value);
    }

    public Value Pop()
    {
        return _stack.Pop();
    }

    public Value Peek()
    {
        return _stack.Peek();
    }

    public void Execute(IEnumerable<ICommand> commands)
    {
        foreach (var command in commands)
        {
            command.Execute(this);
        }
    }

    public void Call(string functionName, int argCount)
    {
        var funcValue = CurrentEnvironment.Load(functionName);

        var args = new Value[argCount];
        for (int i = argCount - 1; i >= 0; i--)
        {
            args[i] = Pop();
        }

        switch (funcValue)
        {
        case ClosureValue closure:
        {
            var previousEnv = CurrentEnvironment;
            CurrentEnvironment = closure.CapturedEnv.Clone();
            CurrentEnvironment.Store(functionName, closure);

            for (int i = 0; i < closure.Parameters.Length; i++)
            {
                CurrentEnvironment.Store(closure.Parameters[i], i < argCount
                    ? args[i]
                    : new IntValue(0, closure.Origin));
            }

            Execute(closure.CompiledBody);
            CurrentEnvironment = previousEnv;
            break;
        }

        case CommandValue commandValue:
            foreach (var arg in args) Push(arg);
            commandValue.Command.Execute(this);
            break;

        default:
            throw new InvalidOperationException($"Identifier '{functionName}' is not callable.");
        }
    }
}
