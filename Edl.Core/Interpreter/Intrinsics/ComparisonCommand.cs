namespace Edl.Core.Interpreter.Intrinsics;

public abstract class ComparisonCommand : ICommand
{
    public void Execute(Context vm)
    {
        var b = vm.Pop();
        var a = vm.Pop();
        int cmp = (a, b) switch
        {
            (IntValue x, IntValue y) => x.Data.CompareTo(y.Data),
            (FloatValue x, FloatValue y) => x.Data.CompareTo(y.Data),
            (StringValue x, StringValue y) => x.Data.CompareTo(y.Data),
            (ListValue x, ListValue y) => (x == y) ? 0 : 1,
            (ClosureValue x, ClosureValue y) => (x == y) ? 0 : 1,
            (CommandValue x, CommandValue y) => (x == y) ? 0 : 1,
            (_, _) => throw new InvalidOperationException($"Cannot compare {a.GetType().Name} and {b.GetType().Name}."),
        };
        vm.Push(new IntValue(Compare(cmp) ? 1 : 0));
    }

    protected abstract bool Compare(int comparisonResult);
}
