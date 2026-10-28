namespace Resturant.Business.Dtos;

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
}
