namespace API.Features.Account.CreateUser;

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
    IReadOnlyList<AccountErrorDto>? Errors = null);
