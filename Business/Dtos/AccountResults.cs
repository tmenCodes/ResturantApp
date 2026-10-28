using Microsoft.AspNetCore.Identity;

namespace Resturant.Business.Dtos;

/// <summary>Why an account creation attempt succeeded or was rejected.</summary>
public enum CreateAccountStatus
{
    Created,
    EmailInUse,
    Rejected
}

/// <summary>
/// The outcome of an account creation attempt. <see cref="UserName"/> and <see cref="Email"/> are set when
/// <see cref="Status"/> is <see cref="CreateAccountStatus.Created"/>; <see cref="Errors"/> when it is
/// <see cref="CreateAccountStatus.Rejected"/>.
/// </summary>
public record CreateAccountResult(
    CreateAccountStatus Status,
    string? UserName = null,
    string? Email = null,
    IReadOnlyList<IdentityError>? Errors = null);

/// <summary>Why a login attempt succeeded or was rejected.</summary>
public enum LoginStatus
{
    Succeeded,
    InvalidCredentials,
    LockedOut
}

/// <summary>The outcome of a login attempt. <see cref="Token"/> is set only when <see cref="Status"/> is <see cref="LoginStatus.Succeeded"/>.</summary>
public record LoginResult(LoginStatus Status, string? Token = null);
