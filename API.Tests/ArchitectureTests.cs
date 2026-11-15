using System.Reflection;
using Resturant.Domain.Cart;
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
    public void Infrastructure_DependsOnlyOnDomain()
    {
        Assert.Equal(["Resturant.Domain"], ProjectReferences(Infrastructure));
    }
}
