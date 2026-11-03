namespace Resturant.Application.Abstractions;

/// <summary>Stores user accounts and verifies their credentials.</summary>
public interface IUserAccountStore
{
    /// <summary>Returns the account registered with this email, or null.</summary>
    Task<UserAccount?> FindByEmailAsync(string email);

    /// <summary>Returns the account registered with this user name, or null.</summary>
    Task<UserAccount?> FindByUserNameAsync(string userName);

    /// <summary>Returns the account with this id, or null.</summary>
    Task<UserAccount?> FindByIdAsync(string userId);

    /// <summary>Returns every registered account.</summary>
    Task<IReadOnlyList<UserAccount>> GetAllAsync();

    /// <summary>Creates an account with exactly the given values. Password rules are enforced by the store.</summary>
    Task<AccountCreationResult> CreateAsync(string fullName, string userName, string email, string password);

    /// <summary>Checks the password of the account with this id. Failed attempts count toward lockout.</summary>
    Task<PasswordCheckResult> CheckPasswordAsync(string userId, string password);
}
