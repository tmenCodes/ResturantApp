namespace Resturant.Application.Abstractions;

/// <summary>Issues signed access tokens that identify a user.</summary>
public interface ITokenIssuer
{
    /// <summary>Returns a signed token carrying the account's id, user name and email.</summary>
    string Issue(UserAccount account);
}
