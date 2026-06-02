namespace Edl.Core.VirtualMachine.Intrinsics.Comparison;

[Intrinsic("<")]
public class LessThanCommand : ComparisonCommand
{
    protected override bool Compare(int comparisonResult) => comparisonResult < 0;
}
