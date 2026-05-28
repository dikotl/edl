namespace Edl.Core.Interpreter.Macros;

[Macro("let")]
public class LetMacro : IMacro
{
    public void Translate(Ast.Call call, Translator t)
    {
        if (call.Arguments.Length != 2)
        {
            throw new Exception("Intrinsic 'let' expects 2 arguments.");
        }

        if (call.Arguments[0] is not Ast.Identifier identifier)
        {
            throw new Exception("First argument to 'let' intrinsic must be an identifier.");
        }

        t.Translate(call.Arguments[1]);
        t.Commands.Add(new StoreCommand(identifier.Name));
    }
}
