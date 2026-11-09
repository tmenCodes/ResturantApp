using Resturant.Domain.Customers;

namespace API.Features.UserProfile.Shared;

/// <summary>A payment method saved to a user's profile.</summary>
public record SavedPaymentMethodDto
{
    public int Id { get; init; }
    public string? Credit { get; init; }
    public string? NameonCard { get; init; }
    public string? CreditCardNumber { get; init; }
    public string? Expiration { get; init; }
    public string? CVV { get; init; }
    public string? UserName { get; init; }

    /// <summary>Copies a saved payment method entity.</summary>
    public static SavedPaymentMethodDto From(SavedPaymentMethod paymentMethod) => new()
    {
        Id = paymentMethod.Id,
        Credit = paymentMethod.Credit,
        NameonCard = paymentMethod.NameonCard,
        CreditCardNumber = paymentMethod.CreditCardNumber,
        Expiration = paymentMethod.Expiration,
        CVV = paymentMethod.CVV,
        UserName = paymentMethod.UserName
    };
}
