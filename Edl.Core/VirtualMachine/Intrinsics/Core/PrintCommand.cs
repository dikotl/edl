namespace Edl.Core.VirtualMachine.Intrinsics.Core;

[Intrinsic("print")]
internal class PrintCommand : IIntrinsic
{
    public void Call(Context context, Value[] args)
    {
        foreach (var arg in args)
        {
            Console.Write(arg.Display());
        }
        context.Push(new IntValue(0));
    }
}
