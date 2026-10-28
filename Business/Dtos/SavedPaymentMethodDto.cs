namespace Resturant.Business.Dtos;

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
}
