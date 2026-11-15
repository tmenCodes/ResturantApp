namespace Resturant.Domain.Payments;

/// <summary>The card details a customer pays with.</summary>
public record CreditCardInformation(string? CardType, string? NameOnCard, string? Number, string? Expiration, string? Cvv);
