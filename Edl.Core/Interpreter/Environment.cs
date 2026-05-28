using System.Diagnostics.CodeAnalysis;

namespace Edl.Core.Interpreter;

public sealed class Environment
{
    private readonly Dictionary<string, Value> _variables = [];

    public void Store(string name, Value value)
    {
        _variables[name] = value;
    }

    public Value Load(string name)
    {
        if (_variables.TryGetValue(name, out var value))
        {
            return value;
        }
        throw new InvalidOperationException($"Variable '{name}' is not defined.");
    }

    public bool TryLoad(string name, [MaybeNullWhen(false)] out Value? value)
    {
        return _variables.TryGetValue(name, out value);
    }

    public Environment Clone()
    {
        var newEnv = new Environment();
        foreach (var kvp in _variables)
        {
            newEnv.Store(kvp.Key, kvp.Value);
        }
        return newEnv;
    }
}
