namespace API.Features.Account.Login;

/// <summary>Why a login attempt succeeded or was rejected.</summary>
public enum LoginStatus
{
    Succeeded,
    InvalidCredentials,
    LockedOut
}

/// <summary>The outcome of a login attempt. <see cref="Token"/> is set only when <see cref="Status"/> is <see cref="LoginStatus.Succeeded"/>.</summary>
public record LoginResult(LoginStatus Status, string? Token = null);
