using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Resturant.Business.Auth;
using Resturant.Data.Identity;

namespace Business.Tests;

public class JwtTokenGeneratorTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    private static readonly JwtSettings Settings = new()
    {
        Secret = "a-test-secret-that-is-at-least-32-characters-long",
        Issuer = "https://issuer.test",
        Audience = "https://audience.test",
        ExpiresInMinutes = 30
    };

    private static readonly WebUser Ada = new() { Id = "user-1", UserName = "ada", Email = "ada@example.com" };

    private static string Generate() =>
        new JwtTokenGenerator(Options.Create(Settings), new FixedTimeProvider(Now)).GenerateToken(Ada);

    [Fact]
    public void GenerateToken_CarriesUserClaimsIssuerAudienceAndExpiry()
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(Generate());

        Assert.Equal(Settings.Issuer, token.Issuer);
        Assert.Equal(Settings.Audience, Assert.Single(token.Audiences));
        Assert.Equal(Now.UtcDateTime.AddMinutes(30), token.ValidTo);
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "user-1");
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Name && c.Value == "ada");
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Email && c.Value == "ada@example.com");
        Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void GenerateToken_IsSignedWithTheConfiguredSecret()
    {
        var parameters = new TokenValidationParameters
        {
            ValidIssuer = Settings.Issuer,
            ValidAudience = Settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret)),
            ValidateLifetime = false
        };

        new JwtSecurityTokenHandler().ValidateToken(Generate(), parameters, out var validated);

        Assert.Equal(SecurityAlgorithms.HmacSha256, ((JwtSecurityToken)validated).Header.Alg);
    }
}
