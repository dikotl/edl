namespace Edl.Core.Shared;

internal abstract class NameAttribute(string name) : Attribute
{
    internal string Name { get; } = name;
}
