using API.Features.Account.CreateUser;

namespace Application.Tests.Features.Account;

public class CreateUserHandlerTests : AccountTestHost
{
    [Fact]
    public async Task HandleAsync_NewEmail_CreatesTrimmedLowerCasedAccount()
    {
        var result = await CreateAsync(" Ada Lovelace ", " Ada ", " Ada@Example.com ");

        Assert.Equal(new CreateAccountResult(CreateAccountStatus.Created, "ada", "ada@example.com"), result);
        var user = Assert.Single(await Store.GetAllAsync());
        Assert.Equal("Ada Lovelace", user.FullName);
    }

    [Fact]
    public async Task HandleAsync_UserNameTaken_AppendsFirstFreeSuffix()
    {
        await CreateAsync("Ada", "ada", "ada1@example.com");
        await CreateAsync("Ada", "ada", "ada2@example.com");

        var result = await CreateAsync("Ada", "ada", "ada3@example.com");

        Assert.Equal("ada2", result.UserName);
    }

    [Fact]
    public async Task HandleAsync_EmailAlreadyRegistered_ReturnsEmailInUse()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");

        var result = await CreateAsync("Grace", "grace", "ADA@example.com");

        Assert.Equal(CreateAccountStatus.EmailInUse, result.Status);
        Assert.Single(await Store.GetAllAsync());
    }

    [Fact]
    public async Task HandleAsync_WeakPassword_ReturnsRejectedWithIdentityErrors()
    {
        var result = await CreateAsync("Ada", "ada", "ada@example.com", "weak");

        Assert.Equal(CreateAccountStatus.Rejected, result.Status);
        Assert.NotEmpty(result.Errors!);
        Assert.Empty(await Store.GetAllAsync());
    }
}
