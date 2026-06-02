using Edl.Core.Shared;

namespace Edl.Core.VirtualMachine;

public sealed class Context
{
    private static readonly Dictionary<string, IIntrinsic> Intrinsics = Registry.Load<IIntrinsic, IntrinsicAttribute>();
    private readonly List<Value> _stack = [];
    private int _stackTop = 0;

    public Environment CurrentEnvironment { get; set; } = new();
    public ClosureValue? CurrentClosure { get; set; } = null;

    public Context()
    {
        foreach (var (intrinsicName, intrinsic) in Intrinsics)
        {
            CurrentEnvironment.Store(intrinsicName, new IntrinsicValue(intrinsic));
        }
    }

    public void Push(Value v)
    {
        if (_stackTop >= _stack.Count)
        {
            _stack.Add(v);
        }
        else
        {
            _stack[_stackTop] = v;
        }
        _stackTop++;
    }

    public Value Pop()
    {
        if (_stackTop <= 0)
        {
            throw new Exception($"execution stack underflow ({_stackTop} <= 0)");
        }
        return _stack[--_stackTop];
    }

    public Value Peek(int distance = 0)
    {
        if (_stackTop - distance <= 0)
        {
            throw new Exception($"execution stack underflow ({_stackTop - distance} <= 0)");
        }
        return _stack[_stackTop - 1 - distance];
    }

    public void Execute(IEnumerable<ICommand> commands)
    {
        foreach (var command in commands)
        {
            // Console.WriteLine(command);
            command.Execute(this);
        }
    }

    public void Call(int argCount)
    {
        var funcValue = Peek(argCount);
        var args = new Value[argCount];

        for (int i = argCount - 1; i >= 0; i--)
        {
            args[i] = Pop();
        }

        // Remove the function value from the stack.
        Pop();

        switch (funcValue)
        {
        case ClosureValue closure:
        {
            if (closure.Parameters.Length != argCount)
            {
                throw new Exception($"invalid arguments count, expected {closure.Parameters.Length}, got {argCount}");
            }

            var previousEnv = CurrentEnvironment;
            var previousClosure = CurrentClosure;
            CurrentEnvironment = closure.CapturedEnv.Clone();

            for (int i = 0; i < closure.Parameters.Length; i++)
            {
                CurrentEnvironment.Store(closure.Parameters[i], args[i]);
            }

            int stackBase = _stack.Count;
            Value? returnValue = null;

            try
            {
                CurrentClosure = closure;
                Execute(closure.CompiledBody);
            }
            catch (ReturnException)
            {
                returnValue = Pop();

                while (_stack.Count > stackBase)
                {
                    Pop();
                }
            }
            finally
            {
                CurrentEnvironment = previousEnv;
                CurrentClosure = previousClosure;
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
