namespace Edl.Core.Interpreter.Intrinsics;

public abstract class ComparisonCommand : IIntrinsic
{
    public void Call(Context context, Value[] args)
    {
        var a = args[0];
        var b = args[1];
        int cmp = (a, b) switch
        {
            (IntValue x, IntValue y) => x.Data.CompareTo(y.Data),
            (FloatValue x, FloatValue y) => x.Data.CompareTo(y.Data),
            (StringValue x, StringValue y) => x.Data.CompareTo(y.Data),
            (ListValue x, ListValue y) => (x == y) ? 0 : 1,
            (ClosureValue x, ClosureValue y) => (x == y) ? 0 : 1,
            (IntrinsicValue x, IntrinsicValue y) => (x == y) ? 0 : 1,
            (_, _) => throw new InvalidOperationException($"Cannot compare {a.GetType().Name} and {b.GetType().Name}."),
        };
        context.Push(new IntValue(Compare(cmp) ? 1 : 0));
    }

    protected abstract bool Compare(int comparisonResult);
}
