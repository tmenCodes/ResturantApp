using API.Features.Account.Login;
using Microsoft.AspNetCore.Identity;

namespace Application.Tests.Features.Account;

public class LoginHandlerTests : AccountTestHost
{
    private LoginHandler Handler => new(Store, new FakeTokenIssuer());

    [Fact]
    public async Task HandleAsync_CorrectPassword_ReturnsToken()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");

        var result = await Handler.HandleAsync("ada@example.com", Password);

        Assert.Equal(new LoginResult(LoginStatus.Succeeded, "token-for-ada"), result);
    }

    [Fact]
    public async Task HandleAsync_WrongPasswordOrUnknownEmail_ReturnsInvalidCredentials()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");

        Assert.Equal(LoginStatus.InvalidCredentials, (await Handler.HandleAsync("ada@example.com", "wrong")).Status);
        Assert.Equal(LoginStatus.InvalidCredentials, (await Handler.HandleAsync("nobody@example.com", Password)).Status);
    }

    [Fact]
    public async Task HandleAsync_TooManyFailures_ReturnsLockedOutEvenWithCorrectPassword()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");
        for (var attempt = 0; attempt < new LockoutOptions().MaxFailedAccessAttempts; attempt++)
        {
            await Handler.HandleAsync("ada@example.com", "wrong");
        }

        var result = await Handler.HandleAsync("ada@example.com", Password);

        Assert.Equal(new LoginResult(LoginStatus.LockedOut), result);
    }
}
