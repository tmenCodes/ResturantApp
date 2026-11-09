using API.Common;
using Resturant.Infrastructure.Identity;

namespace API.Features.Account.FindByUser;

/// <summary>Checks whether a user name is registered.</summary>
public class UserNameExistsHandler : IHandler
{
    private readonly IUserAccountStore _accountStore;

    public UserNameExistsHandler(IUserAccountStore accountStore)
    {
        _accountStore = accountStore;
    }

    /// <summary>Returns true when an account is registered with this user name.</summary>
    public async Task<bool> HandleAsync(string? userName) =>
        userName is not null && await _accountStore.FindByUserNameAsync(userName) is not null;
}
