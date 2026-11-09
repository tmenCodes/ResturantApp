using API.Features.Checkout.CheckoutCart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Services.Ordering;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;

namespace Application.Tests.Features.Checkout;

/// <summary>Shared setup for the checkout handler tests.</summary>
public static class CheckoutTestData
{
    public static readonly DateTimeOffset Now = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    public static CheckoutCartHandler CheckoutHandler(RestaurantContext context)
    {
        var checkoutRepository = new CheckoutRepository(context);
        var eligibility = new CheckoutEligibility(checkoutRepository, new ShoppingCartRepository(context));
        return new CheckoutCartHandler(checkoutRepository, eligibility, new EfUnitOfWork(context), new FixedTimeProvider(Now));
    }

    public static async Task<Guid> AddCartAsync(RestaurantContext context)
    {
        var identity = Guid.NewGuid();
        context.ShoppingCarts.Add(ShoppingCart.Create(identity, Now.UtcDateTime));
        await context.SaveChangesAsync();
        return identity;
    }

    public static CheckoutRequestDto Request(string cartsIdentity) => new()
    {
        Credit = "Visa",
        NameonCard = "Ada Lovelace",
        CreditCardNumber = "4111",
        Expiration = "12/30",
        CVV = "123",
        UserName = "ada",
        CartsIdentity = cartsIdentity
    };
}
