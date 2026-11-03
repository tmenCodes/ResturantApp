using Resturant.Domain.Ordering;

namespace Resturant.Domain.Customers;

/// <summary>A card a user saved to pay with later.</summary>
public class SavedPaymentMethod
{
    private SavedPaymentMethod()
    {
    }

    public int Id { get; private set; }
    public string? Credit { get; private set; }
    public string? NameonCard { get; private set; }
    public string? CreditCardNumber { get; private set; }
    public string? Expiration { get; private set; }
    public string? CVV { get; private set; }
    public string? UserName { get; private set; }

    /// <summary>Saves <paramref name="card"/> for the user named <paramref name="userName"/>.</summary>
    public static SavedPaymentMethod Create(string userName, PaymentCard card)
    {
        var paymentMethod = new SavedPaymentMethod { UserName = userName };
        paymentMethod.UpdateCard(card);
        return paymentMethod;
    }

    /// <summary>Replaces the saved card details with <paramref name="card"/>. The owner does not change.</summary>
    public void UpdateCard(PaymentCard card)
    {
        Credit = card.Credit;
        NameonCard = card.NameOnCard;
        CreditCardNumber = card.Number;
        Expiration = card.Expiration;
        CVV = card.Cvv;
    }
}
