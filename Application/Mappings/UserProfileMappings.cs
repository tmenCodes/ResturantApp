using Resturant.Application.Abstractions;
using Resturant.Application.Dtos;
using Resturant.Domain.Customers;
using Resturant.Domain.Ordering;

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

    public static PaymentCard ToPaymentCard(this UserPaymentInformationDto source) =>
        new(source.CardType, source.NameonCard, source.CreditCardNumber, source.Expiration, source.CVV);

    public static UserOrderHistoryDto ToHistoryDto(this CheckedOutCart cart) =>
        new(cart.TotalPrice, cart.Created.ToString("d"));

    public static UserOrderedItemDto ToDto(this CheckedOutOrderItem item) =>
        new(item.CheckoutTime.ToString(), item.ShoppingCartIdentity.ToString(), item.Name, item.TotalItemPrice.ToString("0.00"));
}
