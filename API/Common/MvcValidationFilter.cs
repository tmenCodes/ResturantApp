using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;

namespace API.Common;

/// <summary>
/// Reproduces <c>[ApiController]</c> model validation, which minimal APIs lack, answering with the same
/// ValidationProblemDetails. Endpoints declare their inputs nullable and opt in explicitly:
/// <list type="bullet">
/// <item>a parameter marked <see cref="RequiredAttribute"/> must be present (MVC's implicit rule for non-nullable parameters);</item>
/// <item>a <c>[FromBody]</c> parameter must have a body, and the body's data annotations must hold.</item>
/// </list>
/// Parameter names are the error keys, so endpoints keep the parameter names the controllers used.
/// </summary>
public static class MvcValidationFilter
{
    private const string EmptyBodyKey = "";
    private const string EmptyBodyMessage = "A non-empty request body is required.";

    private sealed record Check(int Index, string Name, RequiredAttribute? Required, bool IsBody);

    /// <summary>Endpoint filter factory; returns <paramref name="next"/> unchanged when the endpoint has nothing to validate.</summary>
    public static EndpointFilterDelegate Create(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        var checks = context.MethodInfo.GetParameters()
            .Select((parameter, index) => new Check(index, parameter.Name!, parameter.GetCustomAttribute<RequiredAttribute>(), IsBody(parameter)))
            .Where(check => check.Required is not null || check.IsBody)
            .ToArray();
        if (checks.Length == 0)
        {
            return next;
        }

        return async invocation =>
        {
            var errors = Validate(checks, invocation.Arguments);
            return errors.Count > 0 ? Results.ValidationProblem(errors) : await next(invocation);
        };
    }

    private static bool IsBody(ParameterInfo parameter) =>
        parameter.GetCustomAttributes().OfType<IFromBodyMetadata>().Any();

    private static Dictionary<string, string[]> Validate(IEnumerable<Check> checks, IList<object?> arguments)
    {
        var errors = new Dictionary<string, List<string>>();
        foreach (var check in checks)
        {
            var argument = arguments[check.Index];
            if (check.IsBody && argument is null)
            {
                Add(errors, EmptyBodyKey, EmptyBodyMessage);
                Add(errors, check.Name, new RequiredAttribute().FormatErrorMessage(check.Name));
            }
            else if (check.Required is { } required && !required.IsValid(argument))
            {
                Add(errors, check.Name, required.FormatErrorMessage(check.Name));
            }
            else if (check.IsBody)
            {
                AddAnnotationErrors(errors, argument!);
            }
        }
        return errors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }

    private static void AddAnnotationErrors(Dictionary<string, List<string>> errors, object body)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(body, new ValidationContext(body), results, validateAllProperties: true);
        foreach (var result in results)
        {
            foreach (var member in result.MemberNames)
            {
                Add(errors, member, result.ErrorMessage ?? string.Empty);
            }
        }
    }

    private static void Add(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.TryGetValue(key, out var messages))
        {
            errors[key] = messages = [];
        }
        messages.Add(message);
    }
}
