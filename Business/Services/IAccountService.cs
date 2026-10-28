using System.Security.Claims;
using Resturant.Business.Dtos;

namespace Resturant.Business.Services;

/// <summary>Registers users, checks credentials and looks up registered accounts.</summary>
public interface IAccountService
{
    /// <summary>
    /// Creates an account. Email and user name are trimmed and lower-cased; when the user name is taken a numeric
    /// suffix is appended until it is unique. Fails when the email is already registered.
    /// </summary>
    Task<CreateAccountResult> CreateAccountAsync(string fullName, string userName, string email, string? password);

    /// <summary>Checks the password for the account with this email and issues a JWT on success. Failed attempts count toward lockout.</summary>
    Task<LoginResult> LoginAsync(string email, string password);

    /// <summary>Returns true when an account is registered with this email.</summary>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>Returns true when an account is registered with this user name.</summary>
    Task<bool> UserNameExistsAsync(string? userName);

    /// <summary>Returns every registered account.</summary>
    Task<IReadOnlyList<AccountUserDto>> GetAllUsersAsync();

    /// <summary>Returns the account the principal identifies, or null when it no longer exists.</summary>
    Task<AccountUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal);
}
