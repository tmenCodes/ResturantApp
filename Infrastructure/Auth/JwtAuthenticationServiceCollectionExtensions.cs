using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Resturant.Infrastructure.Auth;

/// <summary>Registers JWT bearer authentication from the <see cref="JwtSettings"/> configuration section.</summary>
public static class JwtAuthenticationServiceCollectionExtensions
{
    /// <summary>
    /// Binds <see cref="JwtSettings"/> and makes JWT bearer the default authenticate and challenge scheme, validating
    /// tokens with the same settings <see cref="JwtTokenIssuer"/> signs them with.
    /// </summary>
    /// <exception cref="InvalidOperationException">The "JwtSettings" section is missing.</exception>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtSettings.SectionName);
        var settings = section.Get<JwtSettings>()
                       ?? throw new InvalidOperationException($"The '{JwtSettings.SectionName}' configuration section is missing.");
        services.Configure<JwtSettings>(section);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = settings.CreateValidationParameters());
        return services;
    }
}
