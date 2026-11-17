using API.Features.Account.CreateUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resturant.Infrastructure.Auth;
using Resturant.Infrastructure.Identity;
using Resturant.Infrastructure.Persistence;

namespace API.Tests.Features.Account;

/// <summary>A real ASP.NET Core Identity account store on an in-memory database, for the account handler tests.</summary>
public abstract class AccountTestHost : IDisposable
{
    protected const string Password = "Str0ng!Passw0rd";

    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;

    protected AccountTestHost()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthentication();
        services.AddDbContext<RestaurantContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentityCore<WebUser>()
            .AddRoles<IdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<RestaurantContext>();
        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();

        var scoped = _scope.ServiceProvider;
        Store = new IdentityUserAccountStore(
            scoped.GetRequiredService<UserManager<WebUser>>(),
            scoped.GetRequiredService<SignInManager<WebUser>>());
    }

    protected IUserAccountStore Store { get; }

    /// <summary>The scoped services <see cref="Store"/> was built from, such as the Identity managers.</summary>
    protected IServiceProvider Services => _scope.ServiceProvider;

    protected Task<CreateAccountResult> CreateAsync(string fullName, string userName, string email, string? password = Password) =>
        new CreateUserHandler(Store).HandleAsync(fullName, userName, email, password);

    /// <summary>Creates the role if it is missing and puts the account with this email in it.</summary>
    protected async Task AddToRoleAsync(string email, string role)
    {
        var roles = Services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roles.RoleExistsAsync(role))
        {
            await roles.CreateAsync(new IdentityRole(role));
        }

        var users = Services.GetRequiredService<UserManager<WebUser>>();
        await users.AddToRoleAsync((await users.FindByEmailAsync(email))!, role);
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
        GC.SuppressFinalize(this);
    }

    protected sealed class FakeTokenIssuer : ITokenIssuer
    {
        public string Issue(UserAccount account, IReadOnlyList<string> roles) =>
            roles.Count == 0 ? $"token-for-{account.UserName}" : $"token-for-{account.UserName}[{string.Join(",", roles)}]";
    }
}
