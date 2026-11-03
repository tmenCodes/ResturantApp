using Resturant.Application.Dtos;
using Resturant.Application.Mappings;
using Resturant.Application.Ports.In;
using Resturant.Application.Ports.Out;

namespace Resturant.Application.UseCases;

/// <summary>Default <see cref="IAccountService"/>, backed by an <see cref="IUserAccountStore"/>.</summary>
public class AccountService : IAccountService
{
    private readonly IUserAccountStore _accountStore;
    private readonly ITokenIssuer _tokenIssuer;

    public AccountService(IUserAccountStore accountStore, ITokenIssuer tokenIssuer)
    {
        _accountStore = accountStore;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<CreateAccountResult> CreateAccountAsync(string fullName, string userName, string email, string? password)
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
            : new CreateAccountResult(CreateAccountStatus.Rejected, Errors: result.Errors);
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var account = await _accountStore.FindByEmailAsync(email);
        if (account is null)
        {
            return new LoginResult(LoginStatus.InvalidCredentials);
        }

        return await _accountStore.CheckPasswordAsync(account.Id, password) switch
        {
            PasswordCheckResult.Succeeded => new LoginResult(LoginStatus.Succeeded, _tokenIssuer.Issue(account)),
            PasswordCheckResult.LockedOut => new LoginResult(LoginStatus.LockedOut),
            _ => new LoginResult(LoginStatus.InvalidCredentials)
        };
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _accountStore.FindByEmailAsync(email) is not null;
    }

    public async Task<bool> UserNameExistsAsync(string? userName)
    {
        return userName is not null && await _accountStore.FindByUserNameAsync(userName) is not null;
    }

    public async Task<IReadOnlyList<AccountUserDto>> GetAllUsersAsync()
    {
        var accounts = await _accountStore.GetAllAsync();
        return accounts.Select(account => account.ToDto()).ToList();
    }

    public async Task<AccountUserDto?> GetUserByIdAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var account = await _accountStore.FindByIdAsync(userId);
        return account?.ToDto();
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
