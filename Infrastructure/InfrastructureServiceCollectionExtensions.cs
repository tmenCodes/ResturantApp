using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Resturant.Application.Abstractions;
using Resturant.Domain.Repositories;
using Resturant.Infrastructure.Auth;
using Resturant.Infrastructure.Identity;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;

namespace Resturant.Infrastructure;

/// <summary>Registers the outer-ring implementations of the Domain repositories and Application abstractions.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    private const string MigrationsAssembly = "Resturant.Infrastructure";

    /// <summary>
    /// Adds <see cref="RestaurantContext"/> on PostgreSQL, the repositories, the Identity account store and the JWT
    /// issuer. Safe to call more than once.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<RestaurantContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(MigrationsAssembly)));
        services.AddRepositories();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IUserAccountStore, IdentityUserAccountStore>();
        services.TryAddSingleton<ITokenIssuer, JwtTokenIssuer>();
        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.TryAddScoped<IUnitOfWork, EfUnitOfWork>();
        services.TryAddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.TryAddScoped<IContactRepository, ContactRepository>();
        services.TryAddScoped<ICheckoutRepository, CheckoutRepository>();
        services.TryAddScoped<IMenuRepository, MenuRepository>();
        services.TryAddScoped<IUserProfileRepository, UserProfileRepository>();
    }
}
