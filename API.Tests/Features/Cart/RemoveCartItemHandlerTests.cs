using API.Features.Cart.RemoveCartItem;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;
using static API.Tests.Features.Cart.CartTestData;

namespace API.Tests.Features.Cart;

public class RemoveCartItemHandlerTests
{
    private static RemoveCartItemHandler CreateHandler(RestaurantContext context) =>
        new(context);

    [Fact]
    public async Task HandleAsync_RemovesLineAndRecalculatesTotals()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        await AddAsync(context, cartId);
        await AddAsync(context, cartId, 2);
        var firstLine = await context.OrderItem.OrderBy(x => x.Id).FirstAsync();

        var removed = await CreateHandler(context).HandleAsync(firstLine.Id, cartId);

        Assert.True(removed);
        var cart = await context.ShoppingCarts.Include(x => x.OrderItems).SingleAsync();
        Assert.Single(cart.OrderItems);
        Assert.Equal(20m, cart.SubTotal);
        Assert.Equal(20m + 20m * TaxRate, cart.TotalPrice);
    }

    [Fact]
    public async Task HandleAsync_UnknownItem_ReturnsFalse()
    {
        await using var context = TestDatabase.Create();

        Assert.False(await CreateHandler(context).HandleAsync(12345, Guid.NewGuid()));
    }

    [Fact]
    public async Task HandleAsync_ItemInAnotherCart_ReturnsFalseAndKeepsItem()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        var otherCartId = Guid.NewGuid();
        await AddAsync(context, cartId);
        await AddAsync(context, otherCartId);
        var otherLine = await context.OrderItem.SingleAsync(x => x.ShoppingCartIdentity == otherCartId);

        var removed = await CreateHandler(context).HandleAsync(otherLine.Id, cartId);

        Assert.False(removed);
        Assert.Equal(2, await context.OrderItem.CountAsync());
    }
}
