using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Resturant.Domain.Services.Ordering;

namespace API.Common;

/// <summary>Registers and maps every feature slice in this assembly.</summary>
public static class FeatureRegistration
{
    private static readonly Type[] FeatureTypes = typeof(FeatureRegistration).Assembly.GetTypes()
        .Where(type => type is { IsClass: true, IsAbstract: false })
        .ToArray();

    /// <summary>The concrete <see cref="IHandler"/> types in this assembly.</summary>
    public static IEnumerable<Type> HandlerTypes => FeatureTypes.Where(typeof(IHandler).IsAssignableFrom);

    /// <summary>The concrete <see cref="IEndpoint"/> types in this assembly.</summary>
    public static IEnumerable<Type> EndpointTypes => FeatureTypes.Where(typeof(IEndpoint).IsAssignableFrom);

    /// <summary>
    /// Adds every handler (scoped) and endpoint, the Domain Services they use and the JSON settings
    /// the controllers used. Safe to call more than once.
    /// </summary>
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IOrderItemFactory, OrderItemFactory>();
        services.TryAddScoped<ICheckoutEligibility, CheckoutEligibility>();
        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        foreach (var handler in HandlerTypes)
        {
            services.TryAddScoped(handler);
        }
        foreach (var endpoint in EndpointTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IEndpoint), endpoint));
        }
        return services;
    }

    /// <summary>Maps the route of every registered <see cref="IEndpoint"/>.</summary>
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        foreach (var endpoint in app.ServiceProvider.GetServices<IEndpoint>())
        {
            endpoint.Map(app);
        }
        return app;
    }
}
