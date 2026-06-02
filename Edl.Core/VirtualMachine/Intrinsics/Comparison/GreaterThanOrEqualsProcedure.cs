namespace Edl.Core.VirtualMachine.Intrinsics.Comparison;

[Intrinsic(">=")]
public class GreaterThanOrEqualsProcedure : ComparisonCommand
{
    protected override bool Compare(int comparisonResult) => comparisonResult >= 0;
}
