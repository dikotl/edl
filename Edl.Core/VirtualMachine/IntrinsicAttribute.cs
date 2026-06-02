using Edl.Core.Shared;

namespace Edl.Core.VirtualMachine;

[AttributeUsage(AttributeTargets.Class)]
internal class IntrinsicAttribute(string name) : NameAttribute(name);
