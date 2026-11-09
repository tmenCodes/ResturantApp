using Microsoft.EntityFrameworkCore;
using Resturant.Domain.Exceptions;
using static API.Tests.Features.Cart.CartTestData;

namespace API.Tests.Features.Cart;

public class AddCartItemHandlerTests
{
    [Fact]
    public async Task HandleAsync_NewCart_CreatesCartWithTaxAndReturnsBasePrice()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();

        var result = await AddHandler(context).HandleAsync(Request(cartId, 2, "Extra Tofu", "Soy Sauce"));

        Assert.Equal("Tofu Stir Fry", result.Name);
        Assert.Equal(10m, result.Price);
        var cart = await context.ShoppingCarts.Include(x => x.OrderItems).ThenInclude(x => x.Options).SingleAsync();
        Assert.Equal(cartId, cart.Identity);
        Assert.Equal(Now.UtcDateTime, cart.Created);
        var item = Assert.Single(cart.OrderItems);
        Assert.Equal(23m, item.TotalItemPrice);
        Assert.Equal("start", item.Status);
        Assert.Equal(2, item.Options.Count);
        Assert.Equal(23m, cart.SubTotal);
        Assert.Equal(23m * TaxRate, cart.TaxAmount);
        Assert.Equal(23m + 23m * TaxRate, cart.TotalPrice);
    }

    [Fact]
    public async Task HandleAsync_ExistingCart_AddsLineRecalculatesTotalsAndReturnsLineTotal()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        var handler = AddHandler(context);
        await handler.HandleAsync(Request(cartId));

        var result = await handler.HandleAsync(Request(cartId, 3, "Extra Tofu"));

        Assert.Equal(33m, result.Price);
        var cart = await context.ShoppingCarts.Include(x => x.OrderItems).SingleAsync();
        Assert.Equal(2, cart.OrderItems.Count);
        Assert.Equal(43m, cart.SubTotal);
        Assert.Equal(43m * TaxRate, cart.TaxAmount);
        Assert.Equal(43m + 43m * TaxRate, cart.TotalPrice);
    }

    [Fact]
    public async Task HandleAsync_UnknownOption_IsIgnored()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);

        await AddHandler(context).HandleAsync(Request(Guid.NewGuid(), 1, "Not An Option"));

        var item = await context.OrderItem.Include(x => x.Options).SingleAsync();
        Assert.Empty(item.Options);
        Assert.Equal(10m, item.TotalItemPrice);
    }

    [Fact]
    public async Task HandleAsync_QuantityNotPositive_ThrowsAndSavesNothing()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => AddHandler(context).HandleAsync(Request(Guid.NewGuid(), 0)));

        Assert.Empty(context.ShoppingCarts);
    }

    [Fact]
    public async Task HandleAsync_UnknownMenuItem_ThrowsAndSavesNothing()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var request = Request(Guid.NewGuid());
        request.Name = "Not On The Menu";

        await Assert.ThrowsAsync<MenuItemNotFoundException>(() => AddHandler(context).HandleAsync(request));

        Assert.Empty(context.ShoppingCarts);
    }
}
