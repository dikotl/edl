namespace Edl.Core.VirtualMachine.Intrinsics.Arithmetics;

[Intrinsic("/")]
public class DivideCommand : ArithmeticsCommand
{
    protected override long IntOp(long x, long y)
        => y == 0 ? throw new InvalidOperationException("division by zero") : x / y;

    protected override double FloatOp(double x, double y)
        => y == 0 ? throw new InvalidOperationException("division by zero") : x / y;
}
