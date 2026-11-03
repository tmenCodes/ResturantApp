namespace Resturant.Domain.Ordering;

/// <summary>Payment recorded for a shopping cart.</summary>
public class Checkout
{
    public int Id { get; set; }
    public string? Credit { get; set; }
    public string? NameonCard { get; set; }
    public string? CreditCardNumber { get; set; }
    public string? Expiration { get; set; }
    public string? CVV { get; set; }

    public bool Paid { get; set; } = false;

    public string? UserProfileName { get; set; }

    public DateTime CheckoutTime { get; set; }

    public Guid ShoppingCartIdentity { get; set; }

    /// <summary>Records the cart as paid by this card at <paramref name="paidAtUtc"/>.</summary>
    public static Checkout PayFor(Guid shoppingCartIdentity, PaymentCard card, string? userName, DateTime paidAtUtc) => new()
    {
        Credit = card.Credit,
        NameonCard = card.NameOnCard,
        CreditCardNumber = card.Number,
        Expiration = card.Expiration,
        CVV = card.Cvv,
        UserProfileName = userName,
        ShoppingCartIdentity = shoppingCartIdentity,
        CheckoutTime = paidAtUtc,
        Paid = true
    };
}
