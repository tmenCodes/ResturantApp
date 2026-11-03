namespace Resturant.Domain.Exceptions;

/// <summary>Base type for errors raised when a request names something that does not exist.</summary>
public abstract class NotFoundException : DomainException
{
    protected NotFoundException(string message) : base(message)
    {
    }
}
