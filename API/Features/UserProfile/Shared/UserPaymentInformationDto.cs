using Resturant.Domain.Payments;

namespace API.Features.UserProfile.Shared;

/// <summary>Card details a signed-in user submits to save or replace their payment method.</summary>
public record UserPaymentInformationDto
{
    public string? CardType { get; init; }
    public string? NameonCard { get; init; }
    public string? CreditCardNumber { get; init; }
    public string? Expiration { get; init; }
    public string? CVV { get; init; }

    public UserPaymentInformationDto(string cardType, string nameonCard, string creditCardNumber, string expiration, string cvv)
    {
        CardType = cardType;
        NameonCard = nameonCard;
        CreditCardNumber = creditCardNumber;
        Expiration = expiration;
        CVV = cvv;
    }

    /// <summary>Returns the submitted details as a domain payment card.</summary>
    public CreditCardInformation ToCreditCardInformation() => new(CardType, NameonCard, CreditCardNumber, Expiration, CVV);
}
