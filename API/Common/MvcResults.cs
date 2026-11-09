namespace API.Common;

/// <summary>
/// Results that produce exactly what the former MVC controllers sent, which minimal API defaults would not:
/// string bodies were <c>text/plain</c> and bodiless errors were ProblemDetails.
/// </summary>
public static class MvcResults
{
    /// <summary>A <c>text/plain</c> message with the given status, as MVC wrote <c>BadRequest("...")</c> and similar.</summary>
    public static IResult Message(int statusCode, string message) => Results.Text(message, statusCode: statusCode);

    /// <summary>A ProblemDetails body with the given status, as <c>[ApiController]</c> wrote <c>NotFound()</c> and similar.</summary>
    public static IResult Problem(int statusCode) => Results.Problem(statusCode: statusCode);

    /// <summary>200 with the value as JSON, or 204 when it is null, as MVC wrote <c>Ok(null)</c>.</summary>
    public static IResult OkOrNoContent<T>(T? value) => value is null ? Results.NoContent() : Results.Ok(value);
}
