using Resturant.Domain.Ordering;

namespace API.Features.Checkout.Shared;

/// <summary>A completed checkout: the payment details recorded for a shopping cart.</summary>
public class CheckoutDto
{
    public int Id { get; set; }
    public string? Credit { get; set; }
    public string? NameonCard { get; set; }
    public string? CreditCardNumber { get; set; }
    public string? Expiration { get; set; }
    public string? CVV { get; set; }
    public bool Paid { get; set; }
    public string? UserProfileName { get; set; }
    public DateTime CheckoutTime { get; set; }
    public Guid ShoppingCartIdentity { get; set; }

    /// <summary>Copies a checkout entity's recorded details.</summary>
    public static CheckoutDto From(CheckoutInformation checkout) => new()
    {
        Id = checkout.Id,
        Credit = checkout.Card.CardType,
        NameonCard = checkout.Card.NameOnCard,
        CreditCardNumber = checkout.Card.Number,
        Expiration = checkout.Card.Expiration,
        CVV = checkout.Card.Cvv,
        Paid = checkout.Paid,
        UserProfileName = checkout.UserProfileName,
        CheckoutTime = checkout.CheckoutTime,
        ShoppingCartIdentity = checkout.ShoppingCartIdentity
    };
}
