using Resturant.Infrastructure.Identity;

namespace Resturant.Infrastructure.Auth;

/// <summary>Issues signed access tokens that identify a user.</summary>
public interface ITokenIssuer
{
    /// <summary>Returns a signed token carrying the account's id, user name, email and one role claim per role.</summary>
    string Issue(UserAccount account, IReadOnlyList<string> roles);
}
