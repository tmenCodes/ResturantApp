using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resturant.Application.Abstractions;

namespace Resturant.Infrastructure.Identity;

/// <summary>ASP.NET Core Identity implementation of <see cref="IUserAccountStore"/>.</summary>
public class IdentityUserAccountStore : IUserAccountStore
{
    private readonly UserManager<WebUser> _userManager;
    private readonly SignInManager<WebUser> _signInManager;

    public IdentityUserAccountStore(UserManager<WebUser> userManager, SignInManager<WebUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<UserAccount?> FindByEmailAsync(string email)
    {
        return ToAccount(await _userManager.FindByEmailAsync(email));
    }

    public async Task<UserAccount?> FindByUserNameAsync(string userName)
    {
        return ToAccount(await _userManager.FindByNameAsync(userName));
    }

    public async Task<UserAccount?> FindByIdAsync(string userId)
    {
        return ToAccount(await _userManager.FindByIdAsync(userId));
    }

    public async Task<IReadOnlyList<UserAccount>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Select(user => ToAccount(user)!).ToList();
    }

    public async Task<AccountCreationResult> CreateAsync(string fullName, string userName, string email, string password)
    {
        var user = new WebUser { FullName = fullName, UserName = userName, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        var errors = result.Errors.Select(error => new AccountError(error.Code, error.Description)).ToList();
        return new AccountCreationResult(result.Succeeded, errors);
    }

    public async Task<PasswordCheckResult> CheckPasswordAsync(string userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return PasswordCheckResult.Failed;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return PasswordCheckResult.Succeeded;
        }
        return result.IsLockedOut ? PasswordCheckResult.LockedOut : PasswordCheckResult.Failed;
    }

    private static UserAccount? ToAccount(WebUser? user) =>
        user is null ? null : new UserAccount(user.Id, user.UserName, user.Email, user.FullName);
}
