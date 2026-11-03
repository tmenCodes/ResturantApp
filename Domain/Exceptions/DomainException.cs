namespace Resturant.Domain.Exceptions;

/// <summary>Base type for errors raised when a request breaks a domain rule.</summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}
