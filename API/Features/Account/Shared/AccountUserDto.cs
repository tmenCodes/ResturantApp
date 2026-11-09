using Resturant.Infrastructure.Identity;

namespace API.Features.Account.Shared;

/// <summary>The public profile of a registered user. Never carries credentials or security stamps.</summary>
public record AccountUserDto(string Id, string? UserName, string? Email, string? FullName)
{
    /// <summary>Copies the public fields of an account.</summary>
    public static AccountUserDto From(UserAccount account) => new(account.Id, account.UserName, account.Email, account.FullName);
}
