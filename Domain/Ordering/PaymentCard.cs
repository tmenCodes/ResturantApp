namespace Resturant.Domain.Ordering;

/// <summary>The card details a customer pays with.</summary>
public record PaymentCard(string? Credit, string? NameOnCard, string? Number, string? Expiration, string? Cvv);
