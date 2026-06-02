using Edl.Core.Shared;

namespace Edl.Core.Translation;

[AttributeUsage(AttributeTargets.Class)]
internal class MacroAttribute(string name) : NameAttribute(name);
