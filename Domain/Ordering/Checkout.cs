namespace Resturant.Domain.Ordering;

/// <summary>Payment recorded for a shopping cart.</summary>
public class Checkout
{
    private Checkout()
    {
    }

    public int Id { get; private set; }
    public string? Credit { get; private set; }
    public string? NameonCard { get; private set; }
    public string? CreditCardNumber { get; private set; }
    public string? Expiration { get; private set; }
    public string? CVV { get; private set; }

    public bool Paid { get; private set; }

    public string? UserProfileName { get; private set; }

    public DateTime CheckoutTime { get; private set; }

    public Guid ShoppingCartIdentity { get; private set; }

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
