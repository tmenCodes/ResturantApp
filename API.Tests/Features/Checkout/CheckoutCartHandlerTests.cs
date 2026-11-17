using API.Features.Checkout.CheckoutCart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using static API.Tests.Features.Checkout.CheckoutTestData;

namespace API.Tests.Features.Checkout;

public class CheckoutCartHandlerTests
{
    [Fact]
    public async Task HandleAsync_ExistingCart_SavesPaidCheckoutAtCurrentTime()
    {
        await using var context = TestDatabase.Create();
        var cart = await AddCartAsync(context);

        var result = await CheckoutHandler(context).HandleAsync(Request(cart.ToString()), "ada");

        Assert.Equal(CheckoutStatus.Created, result.Status);
        var saved = Assert.Single(context.Orders);
        Assert.Equal(saved.Id, result.Checkout!.Id);
        Assert.True(saved.Paid);
        Assert.Equal(cart, saved.ShoppingCartIdentity);
        Assert.Equal(Now.UtcDateTime, saved.CheckoutTime);
        Assert.Equal("ada", result.Checkout.UserProfileName);
    }

    [Fact]
    public async Task HandleAsync_Guest_SavesCheckoutForNobody()
    {
        await using var context = TestDatabase.Create();
        var cart = await AddCartAsync(context);

        var result = await CheckoutHandler(context).HandleAsync(Request(cart.ToString()), userName: null);

        Assert.Equal(CheckoutStatus.Created, result.Status);
        Assert.Null(Assert.Single(context.Orders).UserProfileName);
    }

    [Fact]
    public async Task HandleAsync_CartAlreadyCheckedOut_ReturnsCartAlreadyCheckedOut()
    {
        await using var context = TestDatabase.Create();
        var cart = await AddCartAsync(context);
        var handler = CheckoutHandler(context);
        await handler.HandleAsync(Request(cart.ToString()), "ada");

        var result = await handler.HandleAsync(Request(cart.ToString()), "ada");

        Assert.Equal(CheckoutStatus.CartAlreadyCheckedOut, result.Status);
        Assert.Null(result.Checkout);
        Assert.Single(context.Orders);
    }

    [Fact]
    public async Task HandleAsync_CheckedOutButCartMissing_ReturnsCartAlreadyCheckedOut()
    {
        await using var context = TestDatabase.Create();
        var cart = Guid.NewGuid();
        var card = new CreditCardInformation("Visa", "Ada", "4111", "12/30", "123");
        context.Orders.Add(Order.PayFor(cart, card, "ada", Now.UtcDateTime));
        await context.SaveChangesAsync();

        var result = await CheckoutHandler(context).HandleAsync(Request(cart.ToString()), "ada");

        Assert.Equal(CheckoutStatus.CartAlreadyCheckedOut, result.Status);
        Assert.Single(context.Orders);
    }

    [Fact]
    public async Task HandleAsync_UnknownCart_ReturnsCartNotFound()
    {
        await using var context = TestDatabase.Create();

        var result = await CheckoutHandler(context).HandleAsync(Request(Guid.NewGuid().ToString()), "ada");

        Assert.Equal(CheckoutStatus.CartNotFound, result.Status);
        Assert.Empty(context.Orders);
    }

    [Fact]
    public async Task HandleAsync_MalformedCartIdentity_ReturnsCartNotFound()
    {
        await using var context = TestDatabase.Create();

        var result = await CheckoutHandler(context).HandleAsync(Request("not-a-guid"), "ada");

        Assert.Equal(CheckoutStatus.CartNotFound, result.Status);
        Assert.Empty(context.Orders);
    }
}
