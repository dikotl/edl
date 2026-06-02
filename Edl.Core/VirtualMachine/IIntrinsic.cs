namespace Edl.Core.VirtualMachine;

public interface IIntrinsic
{
    void Call(Context context, Value[] args);
}
