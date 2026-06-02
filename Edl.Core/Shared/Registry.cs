using System.Reflection;

namespace Edl.Core.Shared;

internal class Registry
{
    internal static Dictionary<string, TInterface> Load<TInterface, TAttribute>()
        where TAttribute : NameAttribute
    {
        Dictionary<string, TInterface> instances = [];

        var targetTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(GetLoadableTypes)
            .Where(t => !t.IsAbstract);

        foreach (var type in targetTypes)
        {
            var attribute = type.GetCustomAttribute<TAttribute>();
            if (attribute is null) continue;

            if (typeof(TInterface).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type, nonPublic: true);
                if (instance is null) continue;
                instances.Add(attribute.Name, (TInterface)instance);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Type {type.Name} has {nameof(TAttribute)} but " +
                    $"doesn't implement {nameof(TInterface)}."
                );
            }
        }

        return instances;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t is not null)!;
        }
    }
}
