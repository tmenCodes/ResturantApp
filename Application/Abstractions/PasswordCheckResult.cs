namespace Resturant.Application.Abstractions;

/// <summary>The outcome of checking a user's password.</summary>
public enum PasswordCheckResult
{
    Succeeded,
    Failed,
    LockedOut
}
