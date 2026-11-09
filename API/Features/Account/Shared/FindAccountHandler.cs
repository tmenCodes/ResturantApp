using API.Common;
using Resturant.Infrastructure.Identity;

namespace API.Features.Account.Shared;

/// <summary>Looks up an account by id, typically the signed-in user's.</summary>
public class FindAccountHandler : IHandler
{
    private readonly IUserAccountStore _accountStore;

    public FindAccountHandler(IUserAccountStore accountStore)
    {
        _accountStore = accountStore;
    }

    /// <summary>Returns the account with this id, or null when the id is missing or the account no longer exists.</summary>
    public async Task<AccountUserDto?> HandleAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var account = await _accountStore.FindByIdAsync(userId);
        return account is null ? null : AccountUserDto.From(account);
    }
}
