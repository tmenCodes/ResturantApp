using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Resturant.Infrastructure.Identity;

/// <summary>
/// Startup step that seeds the <see cref="Role"/> role and the super admin account that holds it.
/// The credentials are meant for local deployment only.
/// </summary>
public static class SeedSuperAdmin
{
    /// <summary>The name of the role that can manage the restaurant and see every account.</summary>
    public const string Role = "SuperAdmin";

    /// <summary>The email (and user name) of the seeded super admin account.</summary>
    public const string Email = "admin@gmail.com";

    /// <summary>The password of the seeded super admin account.</summary>
    public const string Password = "Nyknicks1!";

    private const string FullName = "Administrator";

    /// <summary>Runs <see cref="SeedAsync(RoleManager{IdentityRole}, UserManager{WebUser})"/> in its own scope. Call once after the app is built.</summary>
    public static async Task SeedSuperAdminAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        await SeedAsync(
            provider.GetRequiredService<RoleManager<IdentityRole>>(),
            provider.GetRequiredService<UserManager<WebUser>>());
    }

    /// <summary>
    /// Creates the <see cref="Role"/> role, the <see cref="Email"/> account with <see cref="Password"/>
    /// and the link between them, each only if it is missing. Safe to run on every startup.
    /// </summary>
    /// <exception cref="InvalidOperationException">Identity rejected the role, account or password.</exception>
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<WebUser> userManager)
    {
        if (!await roleManager.RoleExistsAsync(Role))
        {
            EnsureSucceeded(await roleManager.CreateAsync(new IdentityRole(Role)));
        }

        var admin = await userManager.FindByEmailAsync(Email);
        if (admin is null)
        {
            admin = new WebUser { UserName = Email, Email = Email, FullName = FullName, EmailConfirmed = true };
            EnsureSucceeded(await userManager.CreateAsync(admin, Password));
        }

        if (!await userManager.IsInRoleAsync(admin, Role))
        {
            EnsureSucceeded(await userManager.AddToRoleAsync(admin, Role));
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            var reasons = string.Join(" ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not seed the super admin: {reasons}");
        }
    }
}
