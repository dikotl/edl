namespace Edl.Core.Interpreter.Intrinsics;

public abstract class ArithmeticsCommand : IIntrinsic
{
    public void Call(Context context, Value[] args)
    {
        var a = args[0];
        var b = args[1];
        context.Push((a, b) switch
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
