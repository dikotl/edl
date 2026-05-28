namespace Edl.Core.Interpreter.Intrinsics.Core;

[Intrinsic("print")]
internal class PrintCommand : ICommand
{
    public void Execute(Context context)
    {
        Console.Write(context.Pop().Display());
    }
}
