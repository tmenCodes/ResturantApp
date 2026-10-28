using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Resturant.Business.Auth;
using Resturant.Business.Dtos;
using Resturant.Business.Services;
using Resturant.Data.Context;
using Resturant.Data.Identity;

namespace Business.Tests;

public class AccountServiceTests : IDisposable
{
    private const string Password = "Str0ng!Passw0rd";

    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthentication();
        services.AddDbContext<RestaurantContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentityCore<WebUser>()
            .AddSignInManager()
            .AddEntityFrameworkStores<RestaurantContext>();
        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();

        var scoped = _scope.ServiceProvider;
        _service = new AccountService(
            scoped.GetRequiredService<UserManager<WebUser>>(),
            scoped.GetRequiredService<SignInManager<WebUser>>(),
            new FakeTokenGenerator());
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }

    private sealed class FakeTokenGenerator : IJwtTokenGenerator
    {
        public string GenerateToken(WebUser user) => $"token-for-{user.UserName}";
    }

    [Fact]
    public async Task CreateAccountAsync_NewEmail_CreatesTrimmedLowerCasedAccount()
    {
        var result = await _service.CreateAccountAsync(" Ada Lovelace ", " Ada ", " Ada@Example.com ", Password);

        Assert.Equal(new CreateAccountResult(CreateAccountStatus.Created, "ada", "ada@example.com"), result);
        var user = Assert.Single(await _service.GetAllUsersAsync());
        Assert.Equal("Ada Lovelace", user.FullName);
    }

    [Fact]
    public async Task CreateAccountAsync_UserNameTaken_AppendsFirstFreeSuffix()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada1@example.com", Password);
        await _service.CreateAccountAsync("Ada", "ada", "ada2@example.com", Password);

        var result = await _service.CreateAccountAsync("Ada", "ada", "ada3@example.com", Password);

        Assert.Equal("ada2", result.UserName);
    }

    [Fact]
    public async Task CreateAccountAsync_EmailAlreadyRegistered_ReturnsEmailInUse()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", Password);

        var result = await _service.CreateAccountAsync("Grace", "grace", "ADA@example.com", Password);

        Assert.Equal(CreateAccountStatus.EmailInUse, result.Status);
        Assert.Single(await _service.GetAllUsersAsync());
    }

    [Fact]
    public async Task CreateAccountAsync_WeakPassword_ReturnsRejectedWithIdentityErrors()
    {
        var result = await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", "weak");

        Assert.Equal(CreateAccountStatus.Rejected, result.Status);
        Assert.NotEmpty(result.Errors!);
        Assert.Empty(await _service.GetAllUsersAsync());
    }

    [Fact]
    public async Task LoginAsync_CorrectPassword_ReturnsToken()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", Password);

        var result = await _service.LoginAsync("ada@example.com", Password);

        Assert.Equal(new LoginResult(LoginStatus.Succeeded, "token-for-ada"), result);
    }

    [Fact]
    public async Task LoginAsync_WrongPasswordOrUnknownEmail_ReturnsInvalidCredentials()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", Password);

        Assert.Equal(LoginStatus.InvalidCredentials, (await _service.LoginAsync("ada@example.com", "wrong")).Status);
        Assert.Equal(LoginStatus.InvalidCredentials, (await _service.LoginAsync("nobody@example.com", Password)).Status);
    }

    [Fact]
    public async Task LoginAsync_TooManyFailures_ReturnsLockedOutEvenWithCorrectPassword()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", Password);
        for (var attempt = 0; attempt < new LockoutOptions().MaxFailedAccessAttempts; attempt++)
        {
            await _service.LoginAsync("ada@example.com", "wrong");
        }

        var result = await _service.LoginAsync("ada@example.com", Password);

        Assert.Equal(new LoginResult(LoginStatus.LockedOut), result);
    }

    [Fact]
    public async Task EmailExistsAsync_And_UserNameExistsAsync_ReportRegisteredAccounts()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", Password);

        Assert.True(await _service.EmailExistsAsync("ada@example.com"));
        Assert.False(await _service.EmailExistsAsync("grace@example.com"));
        Assert.True(await _service.UserNameExistsAsync("ada"));
        Assert.False(await _service.UserNameExistsAsync("grace"));
        Assert.False(await _service.UserNameExistsAsync(null));
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsAccountForPrincipalOrNullWhenUnknown()
    {
        await _service.CreateAccountAsync("Ada", "ada", "ada@example.com", Password);
        var ada = Assert.Single(await _service.GetAllUsersAsync());

        var found = await _service.GetCurrentUserAsync(PrincipalFor(ada.Id));
        var missing = await _service.GetCurrentUserAsync(PrincipalFor(Guid.NewGuid().ToString()));

        Assert.Equal(ada, found);
        Assert.Null(missing);
    }

    private static ClaimsPrincipal PrincipalFor(string userId) =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)], "Test"));
}
