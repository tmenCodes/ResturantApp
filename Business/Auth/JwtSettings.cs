namespace Resturant.Business.Auth;

/// <summary>Signing and lifetime settings for issued JWTs, bound from the "JwtSettings" configuration section.</summary>
public class JwtSettings
{
    /// <summary>The configuration section these settings are read from.</summary>
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public double ExpiresInMinutes { get; set; } = 60;
}
