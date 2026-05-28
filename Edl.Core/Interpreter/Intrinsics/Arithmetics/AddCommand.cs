namespace Edl.Core.Interpreter.Intrinsics.Arithmetics;

[Intrinsic("+")]
public class AddCommand : ArithmeticsCommand
{
    protected override long IntOp(long x, long y) => x + y;
    protected override double FloatOp(double x, double y) => x + y;
}
