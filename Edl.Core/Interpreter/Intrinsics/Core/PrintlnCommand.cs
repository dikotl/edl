namespace Edl.Core.Interpreter.Intrinsics.Core;

[Intrinsic("println")]
internal class PrintlnCommand : ICommand
{
    public void Execute(Context vm)
    {
        Console.WriteLine(vm.Pop().Display());
    }
}
