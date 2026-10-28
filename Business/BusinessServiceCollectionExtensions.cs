using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Resturant.Business.Services;

namespace Resturant.Business;

/// <summary>Registers the Business layer's services with the dependency injection container.</summary>
public static class BusinessServiceCollectionExtensions
{
    /// <summary>Adds every Business service. Safe to call more than once.</summary>
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IContactService, ContactService>();
        services.TryAddScoped<ICheckoutService, CheckoutService>();
        return services;
    }
}
