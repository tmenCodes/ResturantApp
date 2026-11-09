using System.ComponentModel.DataAnnotations;

namespace API.Features.Checkout.CheckoutCart;

/// <summary>Payment details submitted for the shopping cart named by <see cref="CartsIdentity"/>.</summary>
public class CheckoutRequestDto
{
    public string? Credit { get; set; }
    public string? NameonCard { get; set; }
    public string? CreditCardNumber { get; set; }
    public string? Expiration { get; set; }
    public string? CVV { get; set; }
    public string? UserName { get; set; }

    public bool Paid { get; set; } = false;

    [Required]
    public string? CartsIdentity { get; set; }
}
