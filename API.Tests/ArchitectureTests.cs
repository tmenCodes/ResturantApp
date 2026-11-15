using System.Reflection;
using Resturant.Domain.Cart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using Resturant.Domain.Services.Ordering;
using Resturant.Infrastructure.Persistence;

namespace API.Tests;

/// <summary>Guards the project boundaries: Domain depends on nothing, Infrastructure only on Domain.</summary>
public class ArchitectureTests
{
    private static readonly string[] FrameworkPrefixes =
    [
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Microsoft.Extensions.Identity",
        "Microsoft.IdentityModel",
        "System.IdentityModel",
        "Npgsql"
    ];

    private static readonly Assembly DomainModel = typeof(ShoppingCart).Assembly;
    private static readonly Assembly Infrastructure = typeof(RestaurantContext).Assembly;

    private static readonly string[] EntityNamespaces =
    [
        "Resturant.Domain.Menu",
        "Resturant.Domain.Ordering",
        "Resturant.Domain.Customers",
        "Resturant.Domain.Contact"
    ];

    private static IReadOnlyList<string> ProjectReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(x => x.Name!).Where(name => name.StartsWith("Resturant.")).Order().ToList();

    private static IEnumerable<string> FrameworkReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(x => x.Name!).Where(name => FrameworkPrefixes.Any(name.StartsWith));

    [Fact]
    public void DomainModel_DependsOnNoOtherProjectOrFramework()
    {
        Assert.Empty(ProjectReferences(DomainModel));
        Assert.Empty(FrameworkReferences(DomainModel));
    }

    [Fact]
    public void DomainServices_BelongToDomainProject()
    {
        Assert.Same(DomainModel, typeof(OrderItemFactory).Assembly);
        Assert.Same(DomainModel, typeof(CheckoutEligibility).Assembly);
        Assert.Contains(DomainModel.GetTypes(), t => t.Namespace?.StartsWith("Resturant.Domain.Services") == true);
    }

    [Fact]
    public void Infrastructure_DependsOnlyOnDomain()
    {
        Assert.Equal(["Resturant.Domain"], ProjectReferences(Infrastructure));
    }

    [Fact]
    public void RepositoryInterfaces_BelongToDomainModel()
    {
        var repositories = DomainModel.GetTypes().Where(t => t.IsInterface && t.Name.EndsWith("Repository")).ToList();

        Assert.NotEmpty(repositories);
        Assert.All(repositories, t => Assert.Equal("Resturant.Domain.Repositories", t.Namespace));
    }

    [Fact]
    public void Entities_NeverDependOnDomainServices()
    {
        const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        var entities = DomainModel.GetTypes().Where(t => EntityNamespaces.Contains(t.Namespace)).ToList();
        var violations = entities
            .SelectMany(e => e.GetFields(all).Select(f => f.FieldType)
                .Concat(e.GetProperties(all).Select(p => p.PropertyType))
                .Concat(e.GetMethods(all).SelectMany(m => m.GetParameters().Select(p => p.ParameterType).Append(m.ReturnType)))
                .Concat(e.GetConstructors(all).SelectMany(c => c.GetParameters().Select(p => p.ParameterType)))
                .SelectMany(TypeGraph.Flatten)
                .Select(type => (Entity: e, Type: type)))
            .Where(x => TypeGraph.InNamespace(x.Type, "Resturant.Domain.Services"))
            .Select(x => $"{x.Entity.Name} uses {x.Type}");

        Assert.NotEmpty(entities);
        Assert.Empty(violations);
    }
}
