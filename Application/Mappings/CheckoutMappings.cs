using Resturant.Application.Dtos;
using Resturant.Domain.Ordering;
namespace Resturant.Application.Mappings;

internal static class CheckoutMappings
{
    public static CheckoutDto ToDto(this Checkout checkout) => new()
    {
        Id = checkout.Id,
        Credit = checkout.Credit,
        NameonCard = checkout.NameonCard,
        CreditCardNumber = checkout.CreditCardNumber,
        Expiration = checkout.Expiration,
        CVV = checkout.CVV,
        Paid = checkout.Paid,
        UserProfileName = checkout.UserProfileName,
        CheckoutTime = checkout.CheckoutTime,
        ShoppingCartIdentity = checkout.ShoppingCartIdentity
    };
}
