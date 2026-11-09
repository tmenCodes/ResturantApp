using API.Common;
using API.Features.Account.Shared;
using Resturant.Infrastructure.Identity;

namespace API.Features.Account.GetAllUsers;

/// <summary>Lists registered accounts.</summary>
public class GetAllUsersHandler : IHandler
{
    private readonly IUserAccountStore _accountStore;

    public GetAllUsersHandler(IUserAccountStore accountStore)
    {
        _accountStore = accountStore;
    }

    /// <summary>Returns every registered account.</summary>
    public async Task<IReadOnlyList<AccountUserDto>> HandleAsync()
    {
        var accounts = await _accountStore.GetAllAsync();
        return accounts.Select(AccountUserDto.From).ToList();
    }
}
