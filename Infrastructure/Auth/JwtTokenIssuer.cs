using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Resturant.Application.Abstractions;
namespace Resturant.Infrastructure.Auth;

/// <summary>HMAC-SHA256 JWT implementation of <see cref="ITokenIssuer"/>.</summary>
public class JwtTokenIssuer : ITokenIssuer
{
    private readonly JwtSettings _settings;
    private readonly TimeProvider _timeProvider;

    public JwtTokenIssuer(IOptions<JwtSettings> settings, TimeProvider timeProvider)
    {
        _settings = settings.Value;
        _timeProvider = timeProvider;
    }

    public string Issue(UserAccount account)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id),
            new(ClaimTypes.Name, account.UserName ?? string.Empty),
            new(ClaimTypes.Email, account.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: _timeProvider.GetUtcNow().UtcDateTime.AddMinutes(_settings.ExpiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
