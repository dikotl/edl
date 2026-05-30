namespace Edl.Core.Interpreter;

public interface IMacro
{
    void Translate(Ast.Call call, Translator translator);
}
