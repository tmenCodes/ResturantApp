using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Resturant.Application.Interfaces;
using Resturant.Application.Services;

namespace Resturant.Application;

/// <summary>Registers the Application Services with the dependency injection container.</summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>Adds every application service with its interface. Safe to call more than once.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IContactService, ContactService>();
        services.TryAddScoped<ICheckoutService, CheckoutService>();
        services.TryAddScoped<IMenuService, MenuService>();
        services.TryAddScoped<ICartService, CartService>();
        services.TryAddScoped<IUserProfileService, UserProfileService>();
        services.TryAddScoped<IAccountService, AccountService>();
        return services;
    }
}
