namespace Edl.Core.Interpreter.Intrinsics.Core;

[Intrinsic("println")]
internal class PrintlnCommand : IIntrinsic
{
    public void Call(Context _, Value[] args)
    {
        foreach (var arg in args)
        {
            Console.Write(arg.Display());
        }
        Console.WriteLine();
    }
}
