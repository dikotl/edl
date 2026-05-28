namespace Edl.Core.Interpreter.Intrinsics.Comparison;

[Intrinsic("==")]
public class EqualsCommand : ComparisonCommand
{
    protected override bool Compare(int comparisonResult) => comparisonResult == 0;
}
