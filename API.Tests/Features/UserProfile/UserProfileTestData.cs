using API.Features.UserProfile.AddPaymentMethod;
using API.Features.UserProfile.Shared;
using Resturant.Domain.Menu;
using Resturant.Domain.Cart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using CheckoutEntity = Resturant.Domain.Ordering.CheckoutInformation;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;

namespace API.Tests.Features.UserProfile;

/// <summary>Shared setup for the user profile handler tests.</summary>
public static class UserProfileTestData
{
    public static UserPaymentInformationDto Payment(string nameOnCard) => new("Visa", nameOnCard, "4111", "12/30", "123");

    public static AddPaymentMethodHandler AddHandler(RestaurantContext context) =>
        new(new UserProfileRepository(context), new EfUnitOfWork(context));

    public static async Task<Guid> AddCheckedOutCartAsync(RestaurantContext context, string userName, DateTime created, decimal total)
    {
        var identity = Guid.NewGuid();
        var cart = ShoppingCart.Create(identity, created);
        context.ShoppingCarts.Add(cart);
        context.Entry(cart).Property(x => x.TotalPrice).CurrentValue = total;
        context.Checkouts.Add(CheckoutEntity.PayFor(identity, new CreditCardInformation("Visa", null, null, null, null), userName, created));
        await context.SaveChangesAsync();
        return identity;
    }

    public static void AddOrderItem(RestaurantContext context, string name, decimal totalItemPrice, Guid cart)
    {
        var item = OrderItem.Create(new MenuItem(name, string.Empty, totalItemPrice), [], quantity: 1);
        context.OrderItem.Add(item);
        context.Entry(item).Property(x => x.ShoppingCartIdentity).CurrentValue = cart;
    }
}
