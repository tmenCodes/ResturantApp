using System.Reflection;
using Resturant.Application;
using Resturant.Domain.Ordering;

namespace Application.Tests;

/// <summary>Guards the hexagonal boundaries: the core must not depend on adapters or frameworks.</summary>
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

    private static IReadOnlyList<string> ReferencedNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(x => x.Name!).ToList();

    private static IEnumerable<string> FrameworkReferences(Assembly assembly) =>
        ReferencedNames(assembly).Where(name => FrameworkPrefixes.Any(name.StartsWith));

    [Fact]
    public void Domain_DependsOnNoOtherProjectOrFramework()
    {
        var domain = typeof(ShoppingCart).Assembly;

        Assert.Empty(ReferencedNames(domain).Where(name => name.StartsWith("Resturant.")));
        Assert.Empty(FrameworkReferences(domain));
    }

    [Fact]
    public void Application_DependsOnlyOnDomainAndNoFramework()
    {
        var application = typeof(ApplicationServiceCollectionExtensions).Assembly;

        Assert.Equal(["Resturant.Domain"], ReferencedNames(application).Where(name => name.StartsWith("Resturant.")));
        Assert.Empty(FrameworkReferences(application));
    }
}
