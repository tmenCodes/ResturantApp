using System.Reflection;
using API.Common;
using Microsoft.Extensions.DependencyInjection;
using NetArchTest.Rules;

namespace API.Tests;

/// <summary>
/// Guards the vertical slices in <c>API.Features</c>: a slice may use Domain, Infrastructure abstractions,
/// <c>API.Common</c> and its own feature's <c>Shared</c> namespace, never another slice; entities stay behind handlers.
/// </summary>
public class SliceArchitectureTests
{
    private const string FeaturesRoot = "API.Features";
    private const string SharedSegment = "Shared";

    private const BindingFlags AllDeclared =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    private static readonly Assembly Api = typeof(FeatureRegistration).Assembly;

    private static readonly string[] SliceNamespaces = Api.GetTypes()
        .Select(t => t.Namespace)
        .OfType<string>()
        .Where(ns => ns.StartsWith(FeaturesRoot + ".") && ns.Split('.').Length == 4)
        .Distinct()
        .Order()
        .ToArray();

    private static string FeatureOf(string sliceNamespace) => string.Join('.', sliceNamespace.Split('.').Take(3));

    public static TheoryData<string> Slices()
    {
        var data = new TheoryData<string>();
        foreach (var slice in SliceNamespaces)
        {
            data.Add(slice);
        }
        return data;
    }

    [Fact]
    public void Features_AreOrganizedIntoSlices()
    {
        var misplaced = Api.GetTypes()
            .Where(t => t.Namespace?.StartsWith(FeaturesRoot) == true && t.Namespace.Split('.').Length != 4)
            .Select(t => t.FullName);

        Assert.True(SliceNamespaces.Length > 20, $"Found only {SliceNamespaces.Length} slice namespaces");
        Assert.Empty(misplaced);
    }

    [Fact]
    public void SliceNames_AreNotPrefixesOfEachOther()
    {
        var overlapping = SliceNamespaces
            .SelectMany(a => SliceNamespaces.Where(b => a != b && b.StartsWith(a)).Select(b => $"{a} / {b}"));

        Assert.Empty(overlapping);
    }

    [Theory]
    [MemberData(nameof(Slices))]
    public void Slice_DependsOnNoOtherSlice(string slice)
    {
        var allowedShared = $"{FeatureOf(slice)}.{SharedSegment}";
        var others = SliceNamespaces.Where(ns => ns != slice && ns != allowedShared).ToArray();

        var result = Types.InAssembly(Api).That().ResideInNamespace(slice).ShouldNot().HaveDependencyOnAny(others).GetResult();

        Assert.True(result.IsSuccessful, $"{slice} uses another slice: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Common_DependsOnNoSlice()
    {
        var result = Types.InAssembly(Api).That().ResideInNamespace("API.Common").ShouldNot().HaveDependencyOn(FeaturesRoot).GetResult();

        Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void HandlersAndEndpoints_LiveInSlices()
    {
        var misplaced = FeatureRegistration.HandlerTypes.Concat(FeatureRegistration.EndpointTypes)
            .Where(t => !TypeGraph.InNamespace(t, FeaturesRoot))
            .Select(t => t.FullName);

        Assert.Empty(misplaced);
    }

    [Fact]
    public void AddFeatures_RegistersEveryHandlerAndEndpoint()
    {
        var services = new ServiceCollection().AddFeatures().AddFeatures();

        var handlers = FeatureRegistration.HandlerTypes.ToList();
        var endpoints = services.Where(d => d.ServiceType == typeof(IEndpoint)).Select(d => d.ImplementationType).ToList();

        Assert.NotEmpty(handlers);
        Assert.All(handlers, h => Assert.Single(services, d => d.ServiceType == h));
        Assert.Equal(FeatureRegistration.EndpointTypes.OrderBy(t => t.FullName), endpoints.OrderBy(t => t!.FullName));
    }

    [Fact]
    public void Endpoints_ReachDomainAndInfrastructureOnlyThroughHandlers()
    {
        var violations = FeatureRegistration.EndpointTypes
            .SelectMany(e => e.GetNestedTypes(AllDeclared).Append(e))
            .SelectMany(t => t.GetMethods(AllDeclared).SelectMany(m => m.GetParameters()).Select(p => (Type: t, p.ParameterType)))
            .Where(x => TypeGraph.Flatten(x.ParameterType).Any(IsDomainOrInfrastructure))
            .Select(x => $"{x.Type.FullName} takes {x.ParameterType}");

        Assert.Empty(violations);
    }

    [Fact]
    public void Handlers_ReturnNoDomainOrInfrastructureTypes()
    {
        var violations = FeatureRegistration.HandlerTypes
            .SelectMany(h => h.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => (Handler: h, Method: m)))
            .Where(x => TypeGraph.Flatten(x.Method.ReturnType).Any(IsDomainOrInfrastructure))
            .Select(x => $"{x.Handler.Name}.{x.Method.Name} returns {x.Method.ReturnType}");

        Assert.Empty(violations);
    }

    [Fact]
    public void Dtos_ExposeNoDomainOrInfrastructureTypes()
    {
        var dtos = Api.GetTypes().Where(t => TypeGraph.InNamespace(t, FeaturesRoot)
                                             && !typeof(IHandler).IsAssignableFrom(t)
                                             && !typeof(IEndpoint).IsAssignableFrom(t));
        var violations = dtos
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => (Dto: t, Property: p)))
            .Where(x => TypeGraph.Flatten(x.Property.PropertyType).Any(IsDomainOrInfrastructure))
            .Select(x => $"{x.Dto.Name}.{x.Property.Name} is {x.Property.PropertyType}");

        Assert.Empty(violations);
    }

    private static bool IsDomainOrInfrastructure(Type type) =>
        TypeGraph.InNamespace(type, "Resturant.Domain") || TypeGraph.InNamespace(type, "Resturant.Infrastructure");
}
