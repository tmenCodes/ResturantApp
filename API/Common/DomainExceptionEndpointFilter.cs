using Resturant.Domain.Exceptions;

namespace API.Common;

/// <summary>Turns domain exceptions thrown by an endpoint into HTTP responses: not-found errors become 404 <c>text/plain</c>.</summary>
public sealed class DomainExceptionEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (NotFoundException notFound)
        {
            return MvcResults.Message(StatusCodes.Status404NotFound, notFound.Message);
        }
    }
}
