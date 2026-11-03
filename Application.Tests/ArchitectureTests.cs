using System.Reflection;
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
}
