using Resturant.Domain.Payments;

namespace Resturant.Domain.Ordering;

/// <summary>Payment recorded for a shopping cart.</summary>
public class CheckoutInformation
{
    private CheckoutInformation()
    {
    }

    public int Id { get; private set; }
    public CreditCardInformation Card { get; private set; } = null!;

    public bool Paid { get; private set; }

    public string? UserProfileName { get; private set; }

    public DateTime CheckoutTime { get; private set; }

    public Guid ShoppingCartIdentity { get; private set; }

    /// <summary>Records the cart as paid by this card at <paramref name="paidAtUtc"/>.</summary>
    public static CheckoutInformation PayFor(Guid shoppingCartIdentity, CreditCardInformation card, string? userName, DateTime paidAtUtc) => new()
    {
        Card = card,
        UserProfileName = userName,
        ShoppingCartIdentity = shoppingCartIdentity,
        CheckoutTime = paidAtUtc,
        Paid = true
    };
}
