using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Common;

/// <summary>
/// Marks operations whose endpoint requires authorization (e.g. <c>.RequireAuthorization()</c>) as needing the JWT
/// bearer scheme, so Swagger UI shows a lock and sends the token only on those.
/// </summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
        {
            return;
        }

        var bearer = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
        };
        operation.Security.Add(new OpenApiSecurityRequirement { [bearer] = Array.Empty<string>() });
    }
}
