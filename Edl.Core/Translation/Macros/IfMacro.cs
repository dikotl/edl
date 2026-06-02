using Edl.Core.Parsing;
using Edl.Core.VirtualMachine;

namespace Edl.Core.Translation.Macros;

[Macro("if")]
public class IfMacro : IMacro
{
    public void Translate(Call call, Translator t)
    {
        ValidateArgs(call.Arguments, out var condition, out var thenExpression, out var elseExpression);

        ICommand[] thenBody = InlineIfLambda(thenExpression);
        ICommand[] elseBody = (elseExpression is not null)
                            ? InlineIfLambda(elseExpression)
                            : [new PushCommand(new IntValue(0))];

        t.Translate(condition);
        t.Commands.Add(new IfCommand(thenBody, elseBody));
    }

    private static ICommand[] InlineIfLambda(Expression expression)
    {
        if (expression is Function function)
        {
            return Translator.Translate(function.Body).ToArray();
        }
        return Translator.Translate([expression]).ToArray();
    }

    private static void ValidateArgs(
        Expression[] args,
        out Expression condition,
        out Expression thenExpression,
        out Expression? elseExpression)
    {
        if (args.Length != 2 && args.Length != 4)
        {
            throw new Exception($"Intrinsic 'if' expects 2 or 4 arguments, got {args.Length}.");
        }

        condition = args[0];
        thenExpression = args[1];
        elseExpression = null;

        if (condition is Function)
        {
            throw new Exception($"Intrinsic 'if' expects the first argument not to be a function.");
        }

        if (thenExpression is Function thenBlock && thenBlock.Parameters.Length > 0)
        {
            throw new Exception($"Intrinsic 'if' expects then-block without parameters.");
        }

        if (args.Length == 4)
        {
            elseExpression = args[3];

            if (args[2] is not Identifier identifier || identifier.Name != "else")
            {
                throw new Exception($"Intrinsic 'if' expects third argument to be identifier 'else', got {args[2]}.");
            }
            if (elseExpression is Function elseBlock && elseBlock.Parameters.Length > 0)
            {
                throw new Exception($"Intrinsic 'if' expects else-block without parameters.");
            }
        }
    }
}
