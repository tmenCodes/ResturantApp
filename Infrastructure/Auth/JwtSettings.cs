using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Resturant.Infrastructure.Auth;

/// <summary>
/// Signing and lifetime settings for JWTs, bound from the "JwtSettings" configuration section. Both issuing
/// (<see cref="JwtTokenIssuer"/>) and validating (<see cref="JwtAuthenticationServiceCollectionExtensions"/>) tokens
/// use these settings, so the two always agree.
/// </summary>
public class JwtSettings
{
    /// <summary>The configuration section these settings are read from.</summary>
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public double ExpiresInMinutes { get; set; } = 60;

    /// <summary>Returns the HMAC key derived from <see cref="Secret"/>.</summary>
    public SymmetricSecurityKey CreateSigningKey() => new(Encoding.UTF8.GetBytes(Secret));

    /// <summary>Returns parameters that accept only unexpired tokens signed with this key, issuer and audience.</summary>
    public TokenValidationParameters CreateValidationParameters() => new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Issuer,
        ValidAudience = Audience,
        IssuerSigningKey = CreateSigningKey()
    };
}
