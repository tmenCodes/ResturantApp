using Resturant.Domain.Payments;

namespace Resturant.Domain.Ordering;

/// <summary>A paid order: the shopping cart that was bought, who bought it (null for a guest), when, and the card used.</summary>
public class Order
{
    private Order()
    {
    }

    public int Id { get; private set; }
    public CreditCardInformation Card { get; private set; } = null!;

    public bool Paid { get; private set; }

    public string? UserProfileName { get; private set; }

    public DateTime CheckoutTime { get; private set; }

    public Guid ShoppingCartIdentity { get; private set; }

    /// <summary>Records the cart as paid by this card at <paramref name="paidAtUtc"/>.</summary>
    public static Order PayFor(Guid shoppingCartIdentity, CreditCardInformation card, string? userName, DateTime paidAtUtc) => new()
    {
        Card = card,
        UserProfileName = userName,
        ShoppingCartIdentity = shoppingCartIdentity,
        CheckoutTime = paidAtUtc,
        Paid = true
    };
}
