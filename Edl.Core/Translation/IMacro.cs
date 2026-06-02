using Edl.Core.Parsing;

namespace Edl.Core.Translation;

public interface IMacro
{
    void Translate(Call call, Translator translator);
}
