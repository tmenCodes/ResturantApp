using System.Reflection;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Resturant.Application;
using Resturant.Domain.Ordering;
using Resturant.Domain.Services.Ordering;

namespace Application.Tests;

/// <summary>Guards the onion rings: every dependency points inward, and no inner ring depends on a framework.</summary>
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
    private static readonly Assembly ApplicationServices = typeof(ApplicationServiceCollectionExtensions).Assembly;
    private static readonly Assembly Api = typeof(PaymentController).Assembly;

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
    public void DomainModel_DependsOnNoOtherRingOrFramework()
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
    public void ApplicationServices_DependOnlyOnDomain()
    {
        Assert.Equal(["Resturant.Domain"], ProjectReferences(ApplicationServices));
        Assert.Empty(FrameworkReferences(ApplicationServices));
    }

    [Fact]
    public void RepositoryInterfaces_BelongToDomainModel()
    {
        var repositories = DomainModel.GetTypes().Where(t => t.IsInterface && t.Name.EndsWith("Repository")).ToList();

        Assert.NotEmpty(repositories);
        Assert.All(repositories, t => Assert.Equal("Resturant.Domain.Repositories", t.Namespace));
        Assert.DoesNotContain(ApplicationServices.GetTypes(), t => t.IsInterface && t.Name.EndsWith("Repository"));
    }

    private static IEnumerable<Type> Flatten(Type type)
    {
        yield return type;
        var inner = type.HasElementType ? [type.GetElementType()!] : type.GetGenericArguments();
        foreach (var argument in inner.SelectMany(Flatten))
        {
            yield return argument;
        }
    }

    private static bool InNamespace(Type type, string root) =>
        type.Namespace is { } ns && (ns == root || ns.StartsWith(root + "."));

    private static List<Type> Controllers() =>
        Api.GetTypes().Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract).ToList();

    [Fact]
    public void Controllers_InjectOnlyApplicationInterfacesAndLoggers()
    {
        var controllers = Controllers();
        var violations = controllers
            .SelectMany(c => c.GetConstructors().SelectMany(ctor => ctor.GetParameters()).Select(p => (Controller: c, p.ParameterType)))
            .Where(x => !InNamespace(x.ParameterType, "Resturant.Application.Interfaces")
                        && !InNamespace(x.ParameterType, "Microsoft.Extensions.Logging"))
            .Select(x => $"{x.Controller.Name} injects {x.ParameterType}");

        Assert.NotEmpty(controllers);
        Assert.Empty(violations);
    }

    [Fact]
    public void Controllers_ExposeNoDomainOrInfrastructureTypes()
    {
        var actions = Controllers().SelectMany(c => c.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        var violations = actions
            .SelectMany(m => m.GetParameters().Select(p => p.ParameterType).Append(m.ReturnType).SelectMany(Flatten)
                .Select(type => (Action: m, Type: type)))
            .Where(x => InNamespace(x.Type, "Resturant.Domain") || InNamespace(x.Type, "Resturant.Infrastructure"))
            .Select(x => $"{x.Action.DeclaringType!.Name}.{x.Action.Name} uses {x.Type}");

        Assert.Empty(violations);
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
                .SelectMany(Flatten)
                .Select(type => (Entity: e, Type: type)))
            .Where(x => InNamespace(x.Type, "Resturant.Domain.Services"))
            .Select(x => $"{x.Entity.Name} uses {x.Type}");

        Assert.NotEmpty(entities);
        Assert.Empty(violations);
    }
}
