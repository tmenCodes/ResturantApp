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

        var result = await CheckoutHandler(context).HandleAsync(Request(cart.ToString()));

        Assert.Equal(CheckoutStatus.Created, result.Status);
        var saved = Assert.Single(context.Checkouts);
        Assert.Equal(saved.Id, result.Checkout!.Id);
        Assert.True(saved.Paid);
        Assert.Equal(cart, saved.ShoppingCartIdentity);
        Assert.Equal(Now.UtcDateTime, saved.CheckoutTime);
        Assert.Equal("ada", result.Checkout.UserProfileName);
    }

    [Fact]
    public async Task HandleAsync_CartAlreadyCheckedOut_ReturnsCartAlreadyCheckedOut()
    {
        await using var context = TestDatabase.Create();
        var cart = await AddCartAsync(context);
        var handler = CheckoutHandler(context);
        await handler.HandleAsync(Request(cart.ToString()));

        var result = await handler.HandleAsync(Request(cart.ToString()));

        Assert.Equal(CheckoutStatus.CartAlreadyCheckedOut, result.Status);
        Assert.Null(result.Checkout);
        Assert.Single(context.Checkouts);
    }

    [Fact]
    public async Task HandleAsync_CheckedOutButCartMissing_ReturnsCartAlreadyCheckedOut()
    {
        await using var context = TestDatabase.Create();
        var cart = Guid.NewGuid();
        var card = new CreditCardInformation("Visa", "Ada", "4111", "12/30", "123");
        context.Checkouts.Add(CheckoutInformation.PayFor(cart, card, "ada", Now.UtcDateTime));
        await context.SaveChangesAsync();

        var result = await CheckoutHandler(context).HandleAsync(Request(cart.ToString()));

        Assert.Equal(CheckoutStatus.CartAlreadyCheckedOut, result.Status);
        Assert.Single(context.Checkouts);
    }

    [Fact]
    public async Task HandleAsync_UnknownCart_ReturnsCartNotFound()
    {
        await using var context = TestDatabase.Create();

        var result = await CheckoutHandler(context).HandleAsync(Request(Guid.NewGuid().ToString()));

        Assert.Equal(CheckoutStatus.CartNotFound, result.Status);
        Assert.Empty(context.Checkouts);
    }

    [Fact]
    public async Task HandleAsync_MalformedCartIdentity_ReturnsCartNotFound()
    {
        await using var context = TestDatabase.Create();

        var result = await CheckoutHandler(context).HandleAsync(Request("not-a-guid"));

        Assert.Equal(CheckoutStatus.CartNotFound, result.Status);
        Assert.Empty(context.Checkouts);
    }
}
