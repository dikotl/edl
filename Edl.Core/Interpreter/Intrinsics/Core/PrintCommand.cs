namespace Edl.Core.Interpreter.Intrinsics.Core;

[Intrinsic("print")]
internal class PrintCommand : IIntrinsic
{
    public void Call(Context _, Value[] args)
    {
        foreach (var arg in args)
        {
            Console.Write(arg.Display());
        }
    }
}
