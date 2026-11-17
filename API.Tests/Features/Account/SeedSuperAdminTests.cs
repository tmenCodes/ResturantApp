using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Resturant.Infrastructure.Identity;

namespace API.Tests.Features.Account;

public class SeedSuperAdminTests : AccountTestHost
{
    private UserManager<WebUser> Users => Services.GetRequiredService<UserManager<WebUser>>();

    private Task SeedAsync() =>
        SeedSuperAdmin.SeedAsync(Services.GetRequiredService<RoleManager<IdentityRole>>(), Users);

    [Fact]
    public async Task SeedAsync_EmptyDatabase_CreatesSuperAdminWithPassword()
    {
        await SeedAsync();

        var admin = (await Store.FindByEmailAsync(SeedSuperAdmin.Email))!;
        Assert.Equal(new UserAccount(admin.Id, SeedSuperAdmin.Email, SeedSuperAdmin.Email, "Administrator"), admin);
        Assert.Equal([SeedSuperAdmin.Role], await Store.GetRolesAsync(admin.Id));
        Assert.Equal(PasswordCheckResult.Succeeded, await Store.CheckPasswordAsync(admin.Id, SeedSuperAdmin.Password));
    }

    [Fact]
    public async Task SeedAsync_ExistingAccountWithoutRole_AddsRole()
    {
        await CreateAsync("Someone", "someone", SeedSuperAdmin.Email);

        await SeedAsync();

        var admin = (await Store.FindByEmailAsync(SeedSuperAdmin.Email))!;
        Assert.Equal([SeedSuperAdmin.Role], await Store.GetRolesAsync(admin.Id));
    }

    [Fact]
    public async Task SeedAsync_RunTwice_KeepsOneAccountAndRole()
    {
        await SeedAsync();
        await SeedAsync();

        Assert.Single(Users.Users);
        Assert.Single(Services.GetRequiredService<RoleManager<IdentityRole>>().Roles);
    }

    [Fact]
    public async Task GetRolesAsync_AccountWithoutRoles_ReturnsEmpty()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");

        Assert.Empty(await Store.GetRolesAsync((await Store.FindByEmailAsync("ada@example.com"))!.Id));
    }
}
