using Microsoft.Extensions.DependencyInjection;
using Resturant.Domain.SeedWork;
using Resturant.Infrastructure.Repository;

namespace Resturant.Infrastructure;

public static class ExtensionServiceMethods
{
    public static void AddRepositoryService(this IServiceCollection services)
    {
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        
    }
}