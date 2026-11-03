using System.Reflection;
using Resturant.Domain.Ordering;

namespace Domain.Tests;

/// <summary>Entities change only through their own methods, so their rules (like cart totals) can't be bypassed.</summary>
public class EncapsulationTests
{
    private static readonly string[] EntityNamespaces =
    [
        "Resturant.Domain.Menu",
        "Resturant.Domain.Ordering",
        "Resturant.Domain.Customers",
        "Resturant.Domain.Contact"
    ];

    public static TheoryData<Type> Entities()
    {
        var entities = typeof(ShoppingCart).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && EntityNamespaces.Contains(t.Namespace) && t.GetProperty("Id") is not null);
        var data = new TheoryData<Type>();
        foreach (var entity in entities)
        {
            data.Add(entity);
        }
        return data;
    }

    [Theory]
    [MemberData(nameof(Entities))]
    public void Entity_HasNoPublicSetters(Type entity)
    {
        var settable = entity.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.SetMethod?.IsPublic == true)
            .Select(p => p.Name);

        Assert.Empty(settable);
    }

    [Theory]
    [MemberData(nameof(Entities))]
    public void Entity_ExposesNoMutableLists(Type entity)
    {
        var lists = entity.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            .Select(p => p.Name);

        Assert.Empty(lists);
    }

    [Fact]
    public void Entities_AreDiscovered()
    {
        Assert.True(Entities().Count() >= 9);
    }
}
