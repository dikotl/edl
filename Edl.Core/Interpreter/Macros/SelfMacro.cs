namespace Edl.Core.Interpreter.Macros;

[Macro("self")]
public class SelfMacro : IMacro
{
    public void Translate(Ast.Call call, Translator t)
    {
        t.Commands.Add(new LoadSelfCommand());
        foreach (var arg in call.Arguments)
        {
            t.Translate(arg);
        }
        t.Commands.Add(new CallCommand(call.Arguments.Length));
    }
}
