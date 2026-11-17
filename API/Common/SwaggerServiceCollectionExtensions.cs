using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace API.Common;

/// <summary>Registers the Swagger document with JWT bearer support.</summary>
public static class SwaggerServiceCollectionExtensions
{
    private const string DocumentName = "v1";
    private const string Title = "MyResturantAPI";

    /// <summary>
    /// Adds Swagger generation for the <c>v1</c> document with a JWT bearer security scheme, so Swagger UI shows an
    /// Authorize button and sends the token on endpoints that require authorization.
    /// </summary>
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo { Title = Title, Version = DocumentName });
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste the token from POST api/Login/Login (no 'Bearer ' prefix)."
            });
            options.OperationFilter<AuthorizeOperationFilter>();
        });
}
