namespace Resturant.Business.Dtos;

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
    //   public string?  UserProfileName { get; set; }
}