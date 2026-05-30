namespace Edl.Core.Interpreter;

public sealed class Context
{
    private static readonly Dictionary<string, IIntrinsic> Intrinsics = Registry.Load<IIntrinsic, IntrinsicAttribute>();
    private readonly Stack<Value> _stack = new();

    public Environment CurrentEnvironment { get; set; } = new();

    public Context()
    {
        foreach (var (intrinsicName, intrinsic) in Intrinsics)
        {
            CurrentEnvironment.Store(intrinsicName, new IntrinsicValue(intrinsic));
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

            int stackBase = _stack.Count;
            Value? returnValue = null;

            try
            {
                Execute(closure.CompiledBody);
            }
            catch (ReturnException)
            {
                returnValue = Pop();

                while (_stack.Count > stackBase)
                {
                    _stack.Pop();
                }
            }
            finally
            {
                CurrentEnvironment = previousEnv;
            }

            if (returnValue is not null)
            {
                Push(returnValue);
            }

            break;
        }

        case IntrinsicValue commandValue:
            commandValue.Intrinsic.Call(this, args);
            break;

        default:
            // Just push it's value.
            Push(funcValue);
            break;
        }
    }
}
