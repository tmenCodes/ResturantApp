using Resturant.Domain.Payments;

namespace Resturant.Domain.Customers;

/// <summary>A card a user saved to pay with later.</summary>
public class SavedPaymentMethod
{
    private SavedPaymentMethod()
    {
    }

    public int Id { get; private set; }
    public CreditCardInformation Card { get; private set; } = null!;
    public string? UserName { get; private set; }

    /// <summary>Saves <paramref name="card"/> for the user named <paramref name="userName"/>.</summary>
    public static SavedPaymentMethod Create(string userName, CreditCardInformation card)
    {
        
        var paymentMethod = new SavedPaymentMethod { UserName = userName };
        paymentMethod.UpdateCard(card);
        return paymentMethod;
    }

    /// <summary>Replaces the saved card details with <paramref name="card"/>. The owner does not change.</summary>
    public void UpdateCard(CreditCardInformation card)
    {
        Card = card;
    }
}
