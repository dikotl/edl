namespace Edl.Core.VirtualMachine.Intrinsics.Arithmetics;

[Intrinsic("-")]
public class SubtractCommand : ArithmeticsCommand
{
    protected override long IntOp(long x, long y) => x - y;
    protected override double FloatOp(double x, double y) => x - y;
}
