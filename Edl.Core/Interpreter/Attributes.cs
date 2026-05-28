namespace Edl.Core.Interpreter;

internal abstract class NameAttribute(string name) : Attribute
{
    internal string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Class)]
internal class MacroAttribute(string name) : NameAttribute(name);

[AttributeUsage(AttributeTargets.Class)]
internal class IntrinsicAttribute(string name) : NameAttribute(name);
