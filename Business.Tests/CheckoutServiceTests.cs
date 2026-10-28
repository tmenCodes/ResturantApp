using Resturant.Business.Dtos;
using Resturant.Business.Services;
using Resturant.Data.Context;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Business.Tests;

public class CheckoutServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    private static CheckoutService CreateService(RestaurantContext context) =>
        new(new CheckoutRepository(context), new ShoppingCartRepository(context), new FixedTimeProvider(Now));

    private static async Task<Guid> AddCartAsync(RestaurantContext context)
    {
        var identity = Guid.NewGuid();
        context.ShoppingCarts.Add(new ShoppingCart { Identity = identity, Created = Now.UtcDateTime });
        await context.SaveChangesAsync();
        return identity;
    }

    private static CustomerInformationDTO Request(string cartsIdentity) => new()
    {
        Credit = "Visa",
        NameonCard = "Ada Lovelace",
        CreditCardNumber = "4111",
        Expiration = "12/30",
        CVV = "123",
        UserName = "ada",
        CartsIdentity = cartsIdentity
    };

    [Fact]
    public async Task CreateAsync_ExistingCart_SavesPaidCheckoutAtCurrentTime()
    {
        await using var context = TestDatabase.Create();
        var cart = await AddCartAsync(context);

        var result = await CreateService(context).CreateAsync(Request(cart.ToString()));

        Assert.Equal(CheckoutStatus.Created, result.Status);
        var saved = Assert.Single(context.Checkouts);
        Assert.Equal(saved.Id, result.Checkout!.Id);
        Assert.True(saved.Paid);
        Assert.Equal(cart, saved.ShoppingCartIdentity);
        Assert.Equal(Now.UtcDateTime, saved.CheckoutTime);
        Assert.Equal("ada", result.Checkout.UserProfileName);
    }

    [Fact]
    public async Task CreateAsync_CartAlreadyCheckedOut_ReturnsCartAlreadyCheckedOut()
    {
        await using var context = TestDatabase.Create();
        var cart = await AddCartAsync(context);
        var service = CreateService(context);
        await service.CreateAsync(Request(cart.ToString()));

        var result = await service.CreateAsync(Request(cart.ToString()));

        Assert.Equal(CheckoutStatus.CartAlreadyCheckedOut, result.Status);
        Assert.Null(result.Checkout);
        Assert.Single(context.Checkouts);
    }

    [Fact]
    public async Task CreateAsync_UnknownCart_ReturnsCartNotFound()
    {
        await using var context = TestDatabase.Create();

        var result = await CreateService(context).CreateAsync(Request(Guid.NewGuid().ToString()));

        Assert.Equal(CheckoutStatus.CartNotFound, result.Status);
        Assert.Empty(context.Checkouts);
    }

    [Fact]
    public async Task CreateAsync_MalformedCartIdentity_ReturnsCartNotFound()
    {
        await using var context = TestDatabase.Create();

        var result = await CreateService(context).CreateAsync(Request("not-a-guid"));

        Assert.Equal(CheckoutStatus.CartNotFound, result.Status);
        Assert.Empty(context.Checkouts);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEveryCheckout()
    {
        await using var context = TestDatabase.Create();
        var service = CreateService(context);
        await service.CreateAsync(Request((await AddCartAsync(context)).ToString()));
        await service.CreateAsync(Request((await AddCartAsync(context)).ToString()));

        var all = await service.GetAllAsync();

        Assert.Equal(2, all.Count);
    }
}
