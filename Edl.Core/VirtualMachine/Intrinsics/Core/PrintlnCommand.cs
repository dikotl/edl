using Edl.Core.VirtualMachine;

namespace Edl.Core.Interpreter.Intrinsics.Core;

[Intrinsic("println")]
internal class PrintlnCommand : IIntrinsic
{
    public void Call(Context context, Value[] args)
    {
        foreach (var arg in args)
        {
            Console.Write(arg.Display());
        }
        Console.WriteLine();
        context.Push(new IntValue(0));
    }
}
