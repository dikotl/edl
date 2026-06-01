namespace Edl.Core.Interpreter.Macros;

[Macro("foreach")]
public class ForeachMacro : IMacro
{
    public void Translate(Ast.Call call, Translator t)
    {
        ValidateArgs(call.Arguments, out var iterable, out var body);

        t.Translate(body);
        t.Translate(iterable);
        t.Commands.Add(new ForeachCommand());
    }

    private static void ValidateArgs(Ast.Expression[] args, out Ast.Expression iterable, out Ast.Expression body)
    {
        if (args.Length != 4)
        {
            throw new Exception($"Intrinsic 'foreach' expects 4 arguments, got {args.Length}.");
        }

        if (args[0] is not Ast.Identifier item)
        {
            throw new Exception($"Intrinsic 'foreach' expects first argument to be an identifier.");
        }

        if (args[1] is not Ast.Identifier identifier || identifier.Name != "in")
        {
            throw new Exception($"Intrinsic 'foreach' expects second argument to be identifier 'in', got {args[1]}.");
        }

        if (args[2] is Ast.Function)
        {
            throw new Exception($"Intrinsic 'foreach' expects third argument not to be a function.");
        }

        if (args[3] is not Ast.Function bodyFunction || bodyFunction.Parameters.Length > 0)
        {
            throw new Exception($"Intrinsic 'foreach' expects last argument to be a function without parameters.");
        }

        iterable = args[2];
        body = new Ast.Function()
        {
            Parameters = [item],
            Body = bodyFunction.Body,
            Location = bodyFunction.Location,
        };
    }
}
