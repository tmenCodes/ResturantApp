using API.Common;
using Resturant.Infrastructure.Identity;

namespace API.Features.Account.CheckDuplicateEmail;

/// <summary>Checks whether an email is registered.</summary>
public class EmailExistsHandler : IHandler
{
    private readonly IUserAccountStore _accountStore;

    public EmailExistsHandler(IUserAccountStore accountStore)
    {
        _accountStore = accountStore;
    }

    /// <summary>Returns true when an account is registered with this email.</summary>
    public async Task<bool> HandleAsync(string email) => await _accountStore.FindByEmailAsync(email) is not null;
}
