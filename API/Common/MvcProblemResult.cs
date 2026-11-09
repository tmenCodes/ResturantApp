using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.Common;

/// <summary>
/// Writes a ProblemDetails body exactly as <c>[ApiController]</c> did: MVC's type and title for the status, a
/// <c>traceId</c>, and <c>application/problem+json; charset=utf-8</c>. Minimal API problem results omit the charset.
/// </summary>
public sealed class MvcProblemResult : IResult
{
    private const string ContentType = "application/problem+json; charset=utf-8";
    private const string ValidationTitle = "One or more validation errors occurred.";

    private static readonly Dictionary<int, (string Type, string Title)> ClientErrors = new()
    {
        [StatusCodes.Status400BadRequest] = ("https://tools.ietf.org/html/rfc9110#section-15.5.1", "Bad Request"),
        [StatusCodes.Status401Unauthorized] = ("https://tools.ietf.org/html/rfc9110#section-15.5.2", "Unauthorized"),
        [StatusCodes.Status404NotFound] = ("https://tools.ietf.org/html/rfc9110#section-15.5.5", "Not Found")
    };

    private readonly ProblemDetails _problem;

    private MvcProblemResult(ProblemDetails problem, int statusCode)
    {
        var (type, title) = ClientErrors[statusCode];
        problem.Status = statusCode;
        problem.Type = type;
        problem.Title ??= title;
        _problem = problem;
    }

    /// <summary>A bodiless client error (400, 401 or 404), as MVC wrote <c>NotFound()</c> and similar.</summary>
    public static MvcProblemResult ForStatus(int statusCode) => new(new ProblemDetails(), statusCode);

    /// <summary>A 400 validation problem listing <paramref name="errors"/> by field.</summary>
    public static MvcProblemResult ForValidation(IDictionary<string, string[]> errors) =>
        new(new ValidationProblemDetails(errors) { Title = ValidationTitle }, StatusCodes.Status400BadRequest);

    public Task ExecuteAsync(HttpContext httpContext)
    {
        _problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = _problem.Status!.Value;
        return httpContext.Response.WriteAsJsonAsync(_problem, _problem.GetType(), options: null, ContentType);
    }
}
