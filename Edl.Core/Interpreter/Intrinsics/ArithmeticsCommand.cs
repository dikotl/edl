namespace Edl.Core.Interpreter.Intrinsics;

public abstract class ArithmeticsCommand : ICommand
{
    public void Execute(Context vm)
    {
        var b = vm.Pop();
        var a = vm.Pop();
        vm.Push((a, b) switch
        {
            (IntValue x, IntValue y) => new IntValue(IntOp(x.Data, y.Data)),
            (FloatValue x, FloatValue y) => new FloatValue(FloatOp(x.Data, y.Data)),
            (FloatValue x, IntValue y) => new FloatValue(FloatOp(x.Data, y.Data)),
            (IntValue x, FloatValue y) => new FloatValue(FloatOp(x.Data, y.Data)),
            (_, _) => throw new InvalidOperationException($"Cannot perform numerical operations on {a.GetType().Name} and {b.GetType().Name}."),
        });
    }

    protected abstract long IntOp(long x, long y);
    protected abstract double FloatOp(double x, double y);
}
