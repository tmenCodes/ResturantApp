namespace Application.Tests;

/// <summary>Reflection helpers shared by the architecture tests.</summary>
public static class TypeGraph
{
    /// <summary>Returns the type plus, recursively, its element type or generic arguments.</summary>
    public static IEnumerable<Type> Flatten(Type type)
    {
        yield return type;
        var inner = type.HasElementType ? [type.GetElementType()!] : type.GetGenericArguments();
        foreach (var argument in inner.SelectMany(Flatten))
        {
            yield return argument;
        }
    }

    /// <summary>True when the type is in <paramref name="root"/> or one of its child namespaces.</summary>
    public static bool InNamespace(Type type, string root) =>
        type.Namespace is { } ns && (ns == root || ns.StartsWith(root + "."));
}
