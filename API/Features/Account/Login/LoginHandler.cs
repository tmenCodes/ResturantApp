using API.Common;
using Resturant.Infrastructure.Auth;
using Resturant.Infrastructure.Identity;

namespace API.Features.Account.Login;

/// <summary>Checks credentials and issues a JWT.</summary>
public class LoginHandler : IHandler
{
    private readonly IUserAccountStore _accountStore;
    private readonly ITokenIssuer _tokenIssuer;

    public LoginHandler(IUserAccountStore accountStore, ITokenIssuer tokenIssuer)
    {
        _accountStore = accountStore;
        _tokenIssuer = tokenIssuer;
    }

    /// <summary>
    /// Checks the password for the account with this email and issues a JWT on success, carrying the roles the account
    /// holds in the database. Failed attempts count toward lockout.
    /// </summary>
    public async Task<LoginResult> HandleAsync(string email, string password)
    {
        var account = await _accountStore.FindByEmailAsync(email);
        if (account is null)
        {
            return new LoginResult(LoginStatus.InvalidCredentials);
        }

        var check = await _accountStore.CheckPasswordAsync(account.Id, password);
        if (check == PasswordCheckResult.Succeeded)
        {
            var roles = await _accountStore.GetRolesAsync(account.Id);
            return new LoginResult(LoginStatus.Succeeded, _tokenIssuer.Issue(account, roles));
        }

        return new LoginResult(check == PasswordCheckResult.LockedOut ? LoginStatus.LockedOut : LoginStatus.InvalidCredentials);
    }
}
