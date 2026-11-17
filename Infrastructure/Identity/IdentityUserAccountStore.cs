using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : new UserAccount(user.Id, user.UserName, user.Email, user.FullName);
    }

    public async Task<UserAccount?> FindByUserNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user is null ? null : new UserAccount(user.Id, user.UserName, user.Email, user.FullName);
    }

    public async Task<UserAccount?> FindByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? null : new UserAccount(user.Id, user.UserName, user.Email, user.FullName);
    }

    public async Task<IReadOnlyList<UserAccount>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Select(user => new UserAccount(user.Id, user.UserName, user.Email, user.FullName)).ToList();
    }

    public async Task<AccountCreationResult> CreateAsync(string fullName, string userName, string email, string password)
    {
        var user = new WebUser { FullName = fullName, UserName = userName, Email = email };
        var result = await _userManager.CreateAsync(user, password);
        return new AccountCreationResult(result.Succeeded, result.Errors.ToList());
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

    public async Task<IReadOnlyList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? [] : (await _userManager.GetRolesAsync(user)).ToList();
    }
}
