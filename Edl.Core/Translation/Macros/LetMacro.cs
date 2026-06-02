using Edl.Core.Parsing;
using Edl.Core.VirtualMachine;

namespace Edl.Core.Translation.Macros;

[Macro("let")]
public class LetMacro : IMacro
{
    public void Translate(Call call, Translator t)
    {
        if (call.Arguments.Length != 2)
        {
            throw new Exception("Intrinsic 'let' expects 2 arguments.");
        }

        if (call.Arguments[0] is not Identifier identifier)
        {
            throw new Exception("First argument to 'let' intrinsic must be an identifier.");
        }

        t.Variables[identifier.Name] = VariableSource.Local;
        t.Translate(call.Arguments[1]);
        t.Commands.Add(new StoreCommand(identifier.Name));
    }
}
