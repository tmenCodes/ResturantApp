using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Resturant.Infrastructure.Auth;
using Resturant.Infrastructure.Identity;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Queries;

namespace Resturant.Infrastructure;

/// <summary>Registers the persistence, identity and auth adapters the API features depend on.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    private const string MigrationsAssembly = "Resturant.Infrastructure";

    /// <summary>
    /// Adds <see cref="RestaurantContext"/> on PostgreSQL, the order-history queries, the Identity account store and the JWT
    /// issuer. Safe to call more than once.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<RestaurantContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(MigrationsAssembly)));
        services.TryAddScoped<IOrderHistoryQueries, OrderHistoryQueries>();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IUserAccountStore, IdentityUserAccountStore>();
        services.TryAddSingleton<ITokenIssuer, JwtTokenIssuer>();
        return services;
    }
}
