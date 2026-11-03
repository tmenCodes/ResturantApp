using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Resturant.Infrastructure.Persistence;

namespace Resturant.Infrastructure.Identity;

/// <summary>Registers ASP.NET Core Identity for <see cref="WebUser"/>, stored in <see cref="RestaurantContext"/>.</summary>
public static class IdentityServiceCollectionExtensions
{
    /// <summary>Adds Identity with its API endpoints, sign-in manager and EF Core stores.</summary>
    public static IServiceCollection AddIdentityAccounts(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<WebUser>()
            .AddEntityFrameworkStores<RestaurantContext>()
            .AddApiEndpoints();
        return services;
    }
}
