namespace Edl.Core.Interpreter;

public interface IIntrinsic
{
    void Call(Context context, Value[] args);
}
