using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resturant.Data.Context;
using Resturant.Data.Repositories;

namespace Resturant.Data;

public static class ExtensionServiceMethods
{
    /// <summary>Registers <see cref="RestaurantContext"/> on PostgreSQL and every repository.</summary>
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<RestaurantContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("Resturant.Data")));
        services.AddRepositoryService();
        return services;
    }

    public static void AddRepositoryService(this IServiceCollection services)
    {
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ICheckoutRepository, CheckoutRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

    }
}
