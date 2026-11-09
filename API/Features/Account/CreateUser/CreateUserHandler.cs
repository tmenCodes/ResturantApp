using API.Common;
using Resturant.Infrastructure.Identity;

namespace API.Features.Account.CreateUser;

/// <summary>Registers a user account.</summary>
public class CreateUserHandler : IHandler
{
    private readonly IUserAccountStore _accountStore;

    public CreateUserHandler(IUserAccountStore accountStore)
    {
        _accountStore = accountStore;
    }

    /// <summary>
    /// Creates an account. Email and user name are trimmed and lower-cased; when the user name is taken a numeric
    /// suffix is appended until it is unique. Fails when the email is already registered.
    /// </summary>
    public async Task<CreateAccountResult> HandleAsync(string fullName, string userName, string email, string? password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await _accountStore.FindByEmailAsync(normalizedEmail) is not null)
        {
            return new CreateAccountResult(CreateAccountStatus.EmailInUse);
        }

        var uniqueUserName = await FindUnusedUserNameAsync(userName.Trim().ToLowerInvariant());
        var result = await _accountStore.CreateAsync(fullName.Trim(), uniqueUserName, normalizedEmail, password!);

        return result.Succeeded
            ? new CreateAccountResult(CreateAccountStatus.Created, uniqueUserName, normalizedEmail)
            : new CreateAccountResult(CreateAccountStatus.Rejected, Errors: result.Errors.Select(e => new AccountErrorDto(e.Code, e.Description)).ToList());
    }

    private async Task<string> FindUnusedUserNameAsync(string baseUserName)
    {
        var candidate = baseUserName;
        for (var suffix = 1; await _accountStore.FindByUserNameAsync(candidate) is not null; suffix++)
        {
            candidate = $"{baseUserName}{suffix}";
        }
        return candidate;
    }
}
