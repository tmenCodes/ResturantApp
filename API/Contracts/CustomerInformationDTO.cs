namespace API.Contracts;

/// <summary>Payment details submitted for a shopping cart at checkout.</summary>
public class CustomerInformationDTO
{
    public string? Credit { get; set; }
    public string? NameonCard { get; set; }
    public string? CreditCardNumber { get; set; }
    public string? Expiration { get; set; }
    public string? CVV { get; set; }
    public string? UserName { get; set; }

    public bool Paid { get; set; } = false;
    public string CartsIdentity { get; set; }
}
