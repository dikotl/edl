using Edl.Core.Parsing;
using Edl.Core.VirtualMachine;

namespace Edl.Core.Translation.Macros;

[Macro("return")]
public class ReturnMacro : IMacro
{
    public void Translate(Call call, Translator t)
    {
        if (call.Arguments.Length > 1)
        {
            throw new Exception("Intrinsic 'return' expects 0 or 1 argument.");
        }

        if (call.Arguments.Length == 1)
        {
            t.Translate(call.Arguments[0]);
        }
        else
        {
            t.Commands.Add(new PushCommand(new IntValue(0)));
        }

        t.Commands.Add(new ReturnCommand());
    }
}
