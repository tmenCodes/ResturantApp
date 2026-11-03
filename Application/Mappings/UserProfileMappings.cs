using Resturant.Application.Abstractions;
using Resturant.Application.Dtos;
using Resturant.Domain.Customers;

namespace Resturant.Application.Mappings;

internal static class UserProfileMappings
{
    public static SavedPaymentMethodDto ToDto(this SavedPaymentMethod paymentMethod) => new()
    {
        Id = paymentMethod.Id,
        Credit = paymentMethod.Credit,
        NameonCard = paymentMethod.NameonCard,
        CreditCardNumber = paymentMethod.CreditCardNumber,
        Expiration = paymentMethod.Expiration,
        CVV = paymentMethod.CVV,
        UserName = paymentMethod.UserName
    };

    public static void ApplyTo(this UserPaymentInformationDto source, SavedPaymentMethod target)
    {
        target.Credit = source.CardType;
        target.NameonCard = source.NameonCard;
        target.CreditCardNumber = source.CreditCardNumber;
        target.Expiration = source.Expiration;
        target.CVV = source.CVV;
    }

    public static UserOrderHistoryDto ToHistoryDto(this CheckedOutCart cart) =>
        new(cart.TotalPrice, cart.Created.ToString("d"));

    public static UserOrderedItemDto ToDto(this CheckedOutOrderItem item) =>
        new(item.CheckoutTime.ToString(), item.ShoppingCartIdentity.ToString(), item.Name, item.TotalItemPrice.ToString("0.00"));
}
