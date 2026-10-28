using Resturant.Data.Identity;

namespace Resturant.Business.Auth;

/// <summary>Issues signed JWTs that identify a user.</summary>
public interface IJwtTokenGenerator
{
    /// <summary>Returns a signed token carrying the user's id, user name and email, valid for the configured lifetime.</summary>
    string GenerateToken(WebUser user);
}
