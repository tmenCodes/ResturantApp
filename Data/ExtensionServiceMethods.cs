using Microsoft.Extensions.DependencyInjection;
using Resturant.Data.Repositories;

namespace Resturant.Data;

public static class ExtensionServiceMethods
{
    public static void AddRepositoryService(this IServiceCollection services)
    {
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ICheckoutRepository, CheckoutRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

    }
}