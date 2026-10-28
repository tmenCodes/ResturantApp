using Microsoft.EntityFrameworkCore;
using Resturant.Business.Dtos;
using Resturant.Business.Services;
using Resturant.Data.Context;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Business.Tests;

public class CartServiceTests
{
    private const decimal TaxRate = 0.06875m;
    private static readonly DateTimeOffset Now = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    private static CartService CreateService(RestaurantContext context) =>
        new(new ShoppingCartRepository(context), new MenuRepository(context), new FixedTimeProvider(Now));

    private static async Task SeedMenuAsync(RestaurantContext context)
    {
        context.MenuItems.Add(new MenuItem { Name = "Tofu Stir Fry", Description = "Tofu", ImageUrl = "Tofu.png", BasePrice = 10m });
        context.MenuItemOptions.AddRange(
            new MenuItemOption { Name = "Extra Tofu", Price = 1m },
            new MenuItemOption { Name = "Soy Sauce", Price = 0.5m });
        await context.SaveChangesAsync();
    }

    private static OrderItemDto Request(Guid cart, int quantity = 1, params string[] options) => new()
    {
        GuidId = cart,
        Name = "Tofu Stir Fry",
        Quantity = quantity,
        Options = options.ToList()
    };

    [Fact]
    public async Task AddItemAsync_NewCart_CreatesCartWithTaxAndReturnsBasePrice()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();

        var result = await CreateService(context).AddItemAsync(Request(cartId, 2, "Extra Tofu", "Soy Sauce"));

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
    public async Task AddItemAsync_ExistingCart_AddsLineRecalculatesTotalsAndReturnsLineTotal()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        var service = CreateService(context);
        await service.AddItemAsync(Request(cartId));

        var result = await service.AddItemAsync(Request(cartId, 3, "Extra Tofu"));

        Assert.Equal(33m, result.Price);
        var cart = await context.ShoppingCarts.Include(x => x.OrderItems).SingleAsync();
        Assert.Equal(2, cart.OrderItems.Count);
        Assert.Equal(43m, cart.SubTotal);
        Assert.Equal(43m * TaxRate, cart.TaxAmount);
        Assert.Equal(43m + 43m * TaxRate, cart.TotalPrice);
    }

    [Fact]
    public async Task AddItemAsync_UnknownOption_IsIgnored()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);

        await CreateService(context).AddItemAsync(Request(Guid.NewGuid(), 1, "Not An Option"));

        var item = await context.OrderItem.Include(x => x.Options).SingleAsync();
        Assert.Empty(item.Options);
        Assert.Equal(10m, item.TotalItemPrice);
    }

    [Fact]
    public async Task AddItemAsync_QuantityNotPositive_ThrowsAndSavesNothing()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateService(context).AddItemAsync(Request(Guid.NewGuid(), 0)));

        Assert.Empty(context.ShoppingCarts);
    }

    [Fact]
    public async Task AddItemAsync_UnknownMenuItem_ThrowsAndSavesNothing()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var request = Request(Guid.NewGuid());
        request.Name = "Not On The Menu";

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateService(context).AddItemAsync(request));

        Assert.Empty(context.ShoppingCarts);
    }

    [Fact]
    public async Task RemoveOrderItemAsync_RemovesLineAndRecalculatesTotals()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        var service = CreateService(context);
        await service.AddItemAsync(Request(cartId));
        await service.AddItemAsync(Request(cartId, 2));
        var firstLine = await context.OrderItem.OrderBy(x => x.Id).FirstAsync();

        var removed = await service.RemoveOrderItemAsync(firstLine.Id, cartId);

        Assert.True(removed);
        var cart = await context.ShoppingCarts.Include(x => x.OrderItems).SingleAsync();
        Assert.Single(cart.OrderItems);
        Assert.Equal(20m, cart.SubTotal);
        Assert.Equal(20m + 20m * TaxRate, cart.TotalPrice);
    }

    [Fact]
    public async Task RemoveOrderItemAsync_UnknownItem_ReturnsFalse()
    {
        await using var context = TestDatabase.Create();

        Assert.False(await CreateService(context).RemoveOrderItemAsync(12345, Guid.NewGuid()));
    }

    [Fact]
    public async Task GetTotalsAsync_FormatsAmountsWithTwoDecimals()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        await CreateService(context).AddItemAsync(Request(cartId, 2, "Extra Tofu", "Soy Sauce"));

        var totals = await CreateService(context).GetTotalsAsync(cartId.ToString());

        Assert.NotNull(totals);
        Assert.Equal(23m.ToString("0.00"), totals.SubTotal);
        Assert.Equal((23m * TaxRate).ToString("0.00"), totals.TaxAmount);
        Assert.Equal((23m + 23m * TaxRate).ToString("0.00"), totals.TotalPrice);
    }

    [Fact]
    public async Task GetTotalsAsync_UnknownCart_ReturnsNull()
    {
        await using var context = TestDatabase.Create();

        Assert.Null(await CreateService(context).GetTotalsAsync(Guid.NewGuid().ToString()));
    }

    [Fact]
    public async Task GetCartItemsAsync_ReturnsLinesWithOptionsAndCount()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        var service = CreateService(context);
        await service.AddItemAsync(Request(cartId, 1, "Extra Tofu"));
        await service.AddItemAsync(Request(Guid.NewGuid()));

        var items = await service.GetCartItemsAsync(cartId.ToString());

        var item = Assert.Single(items);
        Assert.Equal(cartId, item.ShoppingCartIdentity);
        Assert.Equal("Extra Tofu", Assert.Single(item.Options).Name);
        Assert.Equal(1, await service.GetCartItemCountAsync(cartId.ToString()));
        Assert.Equal(2, (await service.GetAllOrderItemsAsync()).Count);
    }

    [Fact]
    public async Task GetCartItemsAsync_MalformedIdentity_ReturnsEmpty()
    {
        await using var context = TestDatabase.Create();

        Assert.Empty(await CreateService(context).GetCartItemsAsync("not-a-guid"));
    }
}
