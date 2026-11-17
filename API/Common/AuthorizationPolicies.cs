using Microsoft.Extensions.DependencyInjection;
using Resturant.Infrastructure.Identity;

namespace API.Common;

/// <summary>Named authorization policies endpoints can require.</summary>
public static class AuthorizationPolicies
{
    /// <summary>Requires the <see cref="SeedSuperAdmin.Role"/> role.</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>Adds authorization with every policy in this class registered.</summary>
    public static IServiceCollection AddRoleAuthorization(this IServiceCollection services) =>
        services.AddAuthorization(options =>
            options.AddPolicy(SuperAdmin, policy => policy.RequireRole(SeedSuperAdmin.Role)));
}
