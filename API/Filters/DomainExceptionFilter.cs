using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Resturant.Domain.Exceptions;

namespace API.Filters;

/// <summary>Turns domain exceptions thrown by an action into HTTP responses: not-found errors become 404.</summary>
public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException notFound)
        {
            context.Result = new NotFoundObjectResult(notFound.Message);
            context.ExceptionHandled = true;
        }
    }
}
