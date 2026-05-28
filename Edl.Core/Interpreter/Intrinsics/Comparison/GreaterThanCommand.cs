namespace Edl.Core.Interpreter.Intrinsics.Comparison;

[Intrinsic(">")]
public class GreaterThanCommand : ComparisonCommand
{
    protected override bool Compare(int comparisonResult) => comparisonResult > 0;
}
