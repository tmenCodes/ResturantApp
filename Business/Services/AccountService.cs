using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resturant.Business.Auth;
using Resturant.Business.Dtos;
using Resturant.Business.Mappings;
using Resturant.Data.Identity;

namespace Resturant.Business.Services;

/// <summary>ASP.NET Core Identity implementation of <see cref="IAccountService"/>.</summary>
public class AccountService : IAccountService
{
    private readonly UserManager<WebUser> _userManager;
    private readonly SignInManager<WebUser> _signInManager;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AccountService(UserManager<WebUser> userManager, SignInManager<WebUser> signInManager, IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<CreateAccountResult> CreateAccountAsync(string fullName, string userName, string email, string? password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await _userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            return new CreateAccountResult(CreateAccountStatus.EmailInUse);
        }

        var uniqueUserName = await FindUnusedUserNameAsync(userName.Trim().ToLowerInvariant());
        var user = new WebUser { FullName = fullName.Trim(), UserName = uniqueUserName, Email = normalizedEmail };
        var result = await _userManager.CreateAsync(user, password!);

        return result.Succeeded
            ? new CreateAccountResult(CreateAccountStatus.Created, uniqueUserName, normalizedEmail)
            : new CreateAccountResult(CreateAccountStatus.Rejected, Errors: result.Errors.ToList());
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new LoginResult(LoginStatus.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            return new LoginResult(LoginStatus.Succeeded, _tokenGenerator.GenerateToken(user));
        }

        return new LoginResult(result.IsLockedOut ? LoginStatus.LockedOut : LoginStatus.InvalidCredentials);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<bool> UserNameExistsAsync(string? userName)
    {
        return userName is not null && await _userManager.FindByNameAsync(userName) is not null;
    }

    public async Task<IReadOnlyList<AccountUserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Select(user => user.ToDto()).ToList();
    }

    public async Task<AccountUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);
        return user?.ToDto();
    }

    private async Task<string> FindUnusedUserNameAsync(string baseUserName)
    {
        var candidate = baseUserName;
        for (var suffix = 1; await _userManager.FindByNameAsync(candidate) is not null; suffix++)
        {
            candidate = $"{baseUserName}{suffix}";
        }
        return candidate;
    }
}
